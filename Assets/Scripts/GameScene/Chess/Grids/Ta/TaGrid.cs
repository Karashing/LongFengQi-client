using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.Networking.Types;

public class TaGrid : XGrid {
    public GameObject hp_go;
    public TMP_Text name_text;
    public TaEffect effect;

    [SerializeField]
    private int _phase; // 0禁止 1龙塔 2禁止 3凤塔 4禁止 5龙凤塔
    public Sprite[] phase_sprites;
    public int phase {
        get { return _phase; }
        set {
            _phase = value;
            hp_go.SetActive(_phase % 2 == 1);
            sprite_renderer.sprite = phase_sprites[_phase % 2];
            if (_phase % 2 == 0) {
                name_text.text = "";
            }
            else {
                if (_phase == 1) name_text.text = "龙塔";
                else if (_phase == 3) name_text.text = "凤塔";
                else name_text.text = "龙凤塔";
            }
        }
    }
    protected override void SetSkills() {
        _skills = new List<XSkill>(){
            new TaSkill0(this, 0),
        };
        _extra_skill = new List<XSkill>(){
            new TaSkill1(this, 3000),
        };
    }

    public override void Init(GridData grid_data) {
        base.Init(grid_data);
        phase = 0;
        EnterActionQueue();
    }
    public override bool CanBeTarget(XTarget target, params XCamp[] able_camps) {
        if (phase % 2 == 0) return false;
        if (hp <= 0) return false;
        return base.CanBeTarget(target, able_camps);
    }
    public override void ActInteractStart(List<XSkill> xskills, bool is_auto = false) {
        base.ActInteractStart(xskills, true);
        var skill = xskills[0];
        if (skill.IsEnable()) {
            skill.BeSelect();
            skill.ConfirmSkill();
        }
    }
    protected override void BeInjured(XActor source_actor) {
        var xskill = extra_skill[0] as TaSkill1;
        xskill.source_actor = source_actor;
        var extra_data = new XExtraData();
        NM.add_extra_actor_action.Send(new(new(this, new List<XSkill> { xskill }, GameInfo.cur_action.round, GameInfo.cur_action.action_id),
                                             new List<XExtraData> { extra_data }));
    }

}