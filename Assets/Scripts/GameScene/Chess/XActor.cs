
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.Events;
using ToolI;
using System.Linq;



public class XActor : BaseBehaviour {
    [ReadOnly]
    public string server_id;
    [HideInInspector]
    public UnityEvent enter_action = new UnityEvent();
    [HideInInspector]
    public UnityEvent quit_action = new UnityEvent();
    [HideInInspector]
    public UnityEvent start_act = new UnityEvent();
    [HideInInspector]
    public UnityEvent end_act = new UnityEvent();
    [HideInInspector]
    public UnityEvent energy_change = new UnityEvent();
    [HideInInspector]
    public UnityEvent level_change = new UnityEvent();
    [HideInInspector]
    public ActionCard action_card;
    public bool in_action {
        get {
            return action_card != null;
        }
    }
    [ReadOnly]
    public int act_round;
    // ----- 说明 -----
    public string word;
    public SymbolType symbol_type;
    public Sprite work_sprite;
    public XDescribe describe;

    // ----- 属性 -----
    public XCamp camp;
    public XCamp opposite_camp {
        get {
            if (camp == XCamp.SELF) return XCamp.ENEMY;
            if (camp == XCamp.ENEMY) return XCamp.SELF;
            return camp;
        }
    }
    public int max_level = 2;
    private int _level; // [0,)
    public int level {
        get { return _level; }
        set {
            var delta_level = value - _level;
            _level = value;
            UpdateLevel(delta_level);
            if (delta_level != 0) level_change.Invoke();
        }
    }
    public int perform_level {
        get { return level + 1; }
    }
    [SerializeField]
    private float _speed;
    public float speed {
        get { return _speed; }
        set {
            _speed = value;
            UpdateActTime();
        }
    }
    public float CalRealSpeedByBuffs() {
        var multi_speed = 0f;
        var delta_speed = 0f;
        foreach (var xbuff in buffs) {
            if (xbuff is ISpeedBuff speed_buff) {
                delta_speed += speed_buff.GetDeltaSpeed();
            }
            if (xbuff is IMultiSpeedBuff multi_speed_buff) {
                multi_speed += multi_speed_buff.GetMultiSpeed();
            }
        }
        return speed * (1 + multi_speed) + delta_speed;
    }
    public float max_energy;
    private float _energy;
    public float energy {
        get { return _energy; }
        set {
            value = Mathf.Min(max_energy, value);
            var flag = _energy != value;
            _energy = value;
            if (flag) energy_change.Invoke();
        }
    }
    [SerializeField]
    [ReadOnly]
    protected float last_act_time;
    [SerializeField]
    [ReadOnly]
    protected float round_mil; // 回合里程
    [ReadOnly]
    public float multi_mil; // 回合计算倍率（推拉条）
    [SerializeField]
    [ReadOnly]
    private float _act_time;
    public float act_time {
        get { return _act_time; }
        set {
            if (value != _act_time) {
                _act_time = value;
                EM.actor_mil_change.Invoke(this);
            }
        }
    }
    public List<XBuff> owner_buffs = new List<XBuff>();
    public TBuff GetBuff<TBuff>() {
        foreach (var xbuff in owner_buffs) {
            if (xbuff is TBuff tbuff) {
                return tbuff;
            }
        }
        return default;
    }
    public void EndBuff<TBuff>() {
        var xbuff = GetBuff<TBuff>() as XBuff;
        xbuff?.End();
    }
    public ListI<XBuff> buffs = new ListI<XBuff>();
    public virtual bool CanBeBuff() {
        return true;
    }
    public virtual bool CanBeBuff<TBuff>() {
        return true;
    }
    public List<TBuff> GetBuffs<TBuff>() {
        var tmp_buffs = new List<TBuff>();
        foreach (var xbuff in buffs) {
            if (xbuff is TBuff tbuff) {
                tmp_buffs.Add(tbuff);
            }
        }
        return tmp_buffs;
    }
    public bool HaveBuff<T>() {
        foreach (var xbuff in buffs) {
            if (xbuff is T) {
                return true;
            }
        }
        return false;
    }
    public void ClearBuffs() {
        var tmp_buffs = new ListI<XBuff>();
        foreach (var xbuff in buffs) {
            tmp_buffs.Add(xbuff);
        }
        foreach (var xbuff in tmp_buffs) {
            xbuff.RemoveTargetActor(this);
        }
    }
    public Dictionary<string, XEffectAction> buff_effect_dict = new Dictionary<string, XEffectAction>();
    public List<KeyValuePair<string, XEffectAction>> GetBuffEffects() {
        List<KeyValuePair<string, XEffectAction>> _buff_effects = new List<KeyValuePair<string, XEffectAction>>();
        foreach (var xbuff in buff_effect_dict) {
            _buff_effects.Add(xbuff);
        }
        return _buff_effects;
    }
    public virtual void RefreshBuffEffect() {
        // speed
        UpdateActTime();

        // buff_effects
        HashSet<string> _effect_names = new HashSet<string>();
        foreach (var xbuff in GetBuffs<XBuff>()) {
            if (xbuff.have_effect) {
                _effect_names.Add(xbuff.buff_effect_name);
            }
        }
        foreach (var xbuff_effect in GetBuffEffects()) {
            if (!_effect_names.Contains(xbuff_effect.Key)) {
                xbuff_effect.Value.Kill();
                buff_effect_dict.Remove(xbuff_effect.Key);
            }
        }
        foreach (var xeffect_names in _effect_names) {
            if (!buff_effect_dict.ContainsKey(xeffect_names)) {
                var effect = FM.LoadEffect(xeffect_names);
                effect.SetParent(transform);
                effect.Play();
                buff_effect_dict.Add(xeffect_names, effect);
            }
        }
    }
    // ----- Skill -----
    public XAction default_action {
        get {
            List<XSkill> tmp_skills = new();
            foreach (var xskill in skills) tmp_skills.Add(xskill);
            foreach (var xskill in functional_skills) tmp_skills.Add(xskill);
            return new XAction(this, tmp_skills, GameInfo.cur_round, GameInfo.cur_action_id) { card = action_card };
        }
    }
    [HideInInspector]
    public List<XSkill> functional_skills = new List<XSkill>();
    protected List<XSkill> _skills = null;
    protected List<XSkill> _extra_skill = null;
    protected List<XSkill> _big_skill = null;
    private bool is_set_skills = false;
    public List<XSkill> skills {
        get {
            if (is_set_skills == false) {
                SetSkills();
                is_set_skills = true;
            }
            return _skills;
        }
    }
    public List<XSkill> big_skill {
        get {
            if (is_set_skills == false) {
                SetSkills();
                is_set_skills = true;
            }
            return _big_skill;
        }
    }
    public List<XSkill> extra_skill {
        get {
            if (is_set_skills == false) {
                SetSkills();
                is_set_skills = true;
            }
            return _extra_skill;
        }
    }
    public XSkill GetSkill(int skill_code) {
        int skill_type = XSkill.GetSkillType(skill_code);
        int skill_id = XSkill.GetSkillId(skill_code);
        if (skill_code < 0) return null;
        else if (skill_type == 0) return skills[skill_id];
        else if (skill_type == 1) return functional_skills[skill_id];
        else if (skill_type == 2) return big_skill[skill_id];
        else if (skill_type == 3) return extra_skill[skill_id];
        return null;
    }

    // ----- Base Fuction ------
    protected virtual void Awake() {
        buffs.SetCallback(RefreshBuffEffect);
    }
    protected virtual void SetSkills() {
        _skills = new List<XSkill>() { };
    }
    public void InitActMil() {
        round_mil = 10000f;
        multi_mil = 1f;
    }
    public void UpdateActTime() { // 上次行动的时间 + (回合里程 * 倍率) / 速度
        act_time = Mathf.Max(GameInfo.cur_act_time, last_act_time + round_mil * multi_mil / CalRealSpeedByBuffs());
        Debug.Log(word + " act_time: " + act_time);
    }

    public virtual void UpdateLevel(int delta_level) { }
    public bool IsCanActByBuffs() {
        bool can_action = true;
        foreach (var action_buff in GetBuffs<IActionBuff>()) {
            can_action &= action_buff.CanAction();
        }
        return can_action;
    }


    // ----- Main Fuction ------
    public virtual void Init() { }
    public virtual void Kill() { }

    public virtual void EnterActionQueue(float initial_mil = 1f) { // 进入行动条
        InitActMil();
        last_act_time = GameInfo.cur_act_time;
        multi_mil = initial_mil;
        UpdateActTime();
        Debug.Log(word + act_time);
        action_card = FM.LoadActionCard(default_action);
        GM.action_queue.AddActor(this);
        GM.big_skill_queue.AddActor(this);
        enter_action.Invoke();
    }
    public virtual void QuitActionQueue() { // 退出行动条
        if (in_action) {
            ClearBuffs();
            GM.action_queue.RemoveActor(this);
            GM.big_skill_queue.RemoveActor(this);
            action_card.End();
            action_card = null;
            quit_action.Invoke();
            Debug.Log("QuitActionQueue");
        }
    }

    public virtual void DestroyGameObject() {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    public void BeAdvanceAction(float advance_rate) {
        multi_mil *= 1 - advance_rate;
        UpdateActTime();
    }




    // ----- Main Fuction ------
    public virtual void ActStart(XAction xaction) { // 外部调用，开始行动
        GameInfo.cur_action = xaction;
        act_round = GameInfo.cur_round;
        InitActMil();
        Debug.Log(word + ": Act Start !");
        GameInfo.SetRoundTimeCallback(0, ActInteractEnd);
        GameInfo.SetRoundTimeCallback(-1, () => { XSkill.ExecuteNoneSkill(server_id, xaction.round, xaction.action_id); });
        GameInfo.SetRoundTime(GameInfo.per_round_time);
        GameInfo.cur_act_actor = this;

        start_act.Invoke();
        GM.profile_panel.Init(this);
        foreach (var xskill in xaction.skills) {
            xskill.Init();
        }

        if (camp == XCamp.SELF || camp == XCamp.NEUTRAL || camp == XCamp.PUBLIC_ENEMY) {
            ActInteractStart(xaction.skills);
        }
    }
    public virtual void ActInteractStart(List<XSkill> xskills, bool is_auto = false) { // 内部执行，具体交互流程
        GameInfo.cur_interact_actor = this;
    }
    public virtual void ActInteractEnd() { // 内部执行，结束/禁止交互
        GameInfo.cur_interact_actor = null;
    }
    public async void ActInteract(int skill_code, XExtraData extra_data = null) { // 外部调用，确认交互技能后
        ActInteractEnd();
        XSkill skill = GetSkill(skill_code);
        if (skill != null) {
            var skill_tip_effect = FM.LoadSkillExecuteTipEffect(this, skill);
            skill_tip_effect.transform.SetParent(GM.detail_panel.transform);
            skill_tip_effect.Play();
            skill.Execute(extra_data);
            await Task.Delay((int)(skill.effect_time() * 1000f));
        }
        // TODO: 确认是否单次交互即回合结束
        ActEnd();
    }
    public void ActEnd() { // 内部调用，在技能释放动画完成之后
        GameInfo.ClearRoundTimeCallback();
        end_act.Invoke();

        GameInfo.cur_act_actor = null;
        last_act_time = GameInfo.cur_act_time;
        UpdateActTime();
        Debug.Log(word + ": Act End !");

        GM.EndAction(GameInfo.cur_action.round, GameInfo.cur_action.action_id);
    }
}