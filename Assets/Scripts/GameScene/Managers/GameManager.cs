using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : BaseBehaviour {
    // public static bool debug_mode = false;
    private static GameManager _instance;
    public static GameManager Ins {
        get {
            return _instance;
        }
    }
    public GameData game_data;
    public GameInfo game_info;
    public GridMap grid_map;
    public ActionQueue action_queue;
    public InteractQueue interact_queue;
    public BeanQueue bean_queue;
    public BigSkillQueue big_skill_queue;
    public ShopModule shop_module;
    public ProfilePanel profile_panel;
    public DetailPanel detail_panel;
    public GameoverPanel gameover_panel;
    public CameraZoomMove camera_moudle;
    void Awake() {
        _instance = this;
        NM.game_load.AddCallback(GameLoad);
        NM.game_start.AddCallback(GameStart);
        NM.game_over.AddCallback(GameOver);

        // NM.next_round.AddCallback((data) => NextRound(data.round));
        NM.next_action.AddCallback((data) => NextAction(data.round, data.action_id));
    }
    private void Start() {
        // GrpcService.Ins.ReiceiveGameMessage((xtype, xcontent) => { EM.grpc_event_buffer.AddGrpcEvent(xtype, xcontent); });
        // GrpcService.Ins.GameMessageStream((xtype, xcontent) => { EM.grpc_event_dict[xtype].GrpcCallback(xcontent); });
    }

    private void GameLoad(ChessBoardData chess_board_data) {
        game_data.Init();
        game_info.Init();
        GameInfo.LoadBoardData(chess_board_data);
        bean_queue.Init();
        NM.game_init.Send();
        Debug.Log("GameLoad Successed");
    }
    private void GameStart() {
        GameInfo.bean = 2;
        GameInfo.coin = 0;
        GameInfo.exp = 1;
        LoadingPanel.Ins.End();
        NextAction(1, 0);
    }
    public async void NextAction(int round, int action_id) { // After Actor ActEnd => Actor Act
        EM.round_switching.Invoke(true);
        var action = GameInfo.NextAction(round, action_id);
        await Task.Delay(500);
        EM.round_switching.Invoke(false);
        Debug.Log("NextAction: " + action.GetActionStr());
        action.actor.ActStart(action);
    }
    public void EndAction(int round, int action_id = 0) {
        if (GameInfo.IsGameOver()) {
            NM.game_end.Send(new(GameInfo.GetGameScore(), GameInfo.cur_act_time));
            return;
        }
        NM.next_action.Send(new NextActionData(round, action_id));
        // action_queue.
    }

    public void GameOver(GameRatingData rating_data) {
        gameover_panel.Init(rating_data);
    }
    public void CloseGame() {
        GrpcService.Ins.QuitGameStream();
        SceneManager.LoadScene(1);
    }


    private void Update() {
        GameInfo.Update();
    }



}