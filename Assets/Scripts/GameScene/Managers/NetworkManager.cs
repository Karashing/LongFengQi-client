using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using ToolI;
using System.Linq;

public class NetworkManager : BaseBehaviour {
    private static NetworkManager _instance;
    public static NetworkManager Ins {
        get {
            return _instance;
        }
    }
    private CancellationTokenSource cts = new CancellationTokenSource();
    public Dictionary<string, IGrpcCallback> grpc_message_dict = new Dictionary<string, IGrpcCallback>();
    public Dictionary<string, AsyncQueue<Tuple<string, string>>> ordered_mq_dict = new Dictionary<string, AsyncQueue<Tuple<string, string>>>(){
        {"ROUND", new AsyncQueue<Tuple<string, string>>()},
        {"ACTOR", new AsyncQueue<Tuple<string, string>>()},
        {"CONVERSATION", new AsyncQueue<Tuple<string, string>>()},
    };


    // ----------------- GrpcMessage -----------------
    public GrpcOrderedMessage<ChessBoardData> game_load = new("GAME_LOAD", "ROUND");
    public GrpcOrderedMessage game_init = new("GAME_INIT", "ROUND");
    public GrpcOrderedMessage game_start = new("GAME_START", "ROUND");
    public GrpcOrderedMessage<GameResultData> game_end = new("GAME_END", "ROUND");
    public GrpcOrderedMessage<GameRatingData> game_over = new("GAME_OVER", "ROUND");
    public GrpcOrderedMessage<ConversationData> conversation = new("CONVERSATION", "CONVERSATION");

    public GrpcOrderedMessage<ActorData> build_actor = new("BUILD_ACTOR", "ACTOR"); // TODO: 本质是shop的interact
    public GrpcOrderedMessage<ActorData> destroy_actor = new("DESTROY_ACTOR", "ACTOR");
    public GrpcOrderedMessage<ActionData> actor_interact = new("ACTOR_INTERACT", "ROUND"); // TODO: 拆分？不包含特殊棋子
    public GrpcOrderedMessage<AddExtraActionData> add_extra_actor_action = new("ADD_EXTRA_ACTOR_ACTION", "ROUND"); // TODO: Actor's ExtraSkill
    // public GrpcOrderedMessage<NextRoundData> next_round;
    public GrpcOrderedMessage<NextActionData> next_action = new("NEXT_ACTION", "ROUND");

    private void Awake(){
        _instance = this;
        List<Task> mq_tasks = new List<Task>();
        foreach(var (message_group, mq) in ordered_mq_dict){
            async Task MQTask(){
                while(cts.IsCancellationRequested == false){
                    var (message_type, json_data) = await mq.Dequeue();
                    grpc_message_dict[message_type].Callback(json_data);
                }
            }
            var mq_task = MQTask();
            mq_tasks.Append(mq_task);
        }
        Task.WhenAll(mq_tasks);

        game_load.Init();
        game_init.Init();
        game_start.Init();
        game_end.Init();
        game_over.Init();
        conversation.Init();
        build_actor.Init();
        destroy_actor.Init();
        actor_interact.Init();
        add_extra_actor_action.Init();
        next_action.Init();

        build_actor.AddCallback(BuildActor);
        destroy_actor.AddCallback(DestroyActor);
        actor_interact.AddCallback(ActorInteract);
        add_extra_actor_action.AddCallback(AddExtraAction);
    }
    private void Start() {
        GrpcService.Ins.GameMessageStream((string message_type, string message_group, string content) => {
            if(!ordered_mq_dict.ContainsKey(message_group)){
                Debug.LogError("ordered_mq_dict has not key: " + message_group);
                return;
            }
            ordered_mq_dict[message_group].Enqueue(new(message_type, content));
        });
    }
    

    void BuildActor(ActorData actor_data) {
        // actors_change.Invoke();
    }
    void DestroyActor(ActorData actor_data) {
        XActor actor;
        if (actor_data.actor_type == ActorType.CHESS) {
            var chess_data = actor_data.LoadData<ChessData>();
            actor = GameInfo.actor_dict[chess_data.server_id];
            actor.Kill();
        }
        else if (actor_data.actor_type == ActorType.GRID) {
            var grid_data = actor_data.LoadData<GridData>();
            actor = GameInfo.actor_dict[grid_data.server_id];
            actor.Kill();
        }
        else {
            return;
        }
        // actors_change.Invoke();
    }
    void ActorInteract(ActionData action_data) {
        var actor = GameInfo.actor_dict[action_data.server_id];
        actor.ActInteract(action_data.skill_code, action_data.extra_data);
    }
    void AddExtraAction(AddExtraActionData data) {
        GM.action_queue.AddAction(data.GetXAction());
    }
}


public interface IGrpcCallback {
    public void Callback(string json_data);
}

public class GrpcOrderedMessage : BaseGame, IGrpcCallback {
    private string content_type, content_group;
    private List<UnityAction> _callbacks = new List<UnityAction>();
    public GrpcOrderedMessage(string xcontent_type, string xcontent_group) {
        content_type = xcontent_type;
        content_group = xcontent_group;
    }
    public void Init(){
        NM.grpc_message_dict.Add(content_type, this);
    }
    public void AddCallback(UnityAction call) {
        _callbacks.Add(call);
    }
    public void Send() {
        GrpcService.Ins.SendGameMessage(content_type, content_group, "");
    }
    public void Callback(string json_data) {
        // after grpc and async_mq
        foreach(var callback in _callbacks){
            callback();
        }
    }
}
public class GrpcOrderedMessage<T> : BaseGame, IGrpcCallback {
    private string content_type, content_group;
    private List<UnityAction<T>> _callbacks = new List<UnityAction<T>>();
    public GrpcOrderedMessage(string xcontent_type, string xcontent_group) {
        content_type = xcontent_type;
        content_group = xcontent_group;
    }
    public void Init(){
        NM.grpc_message_dict.Add(content_type, this);
    }
    public void AddCallback(UnityAction<T> call) {
        _callbacks.Add(call);
    }
    public void Send(T serializable_obj) {
        GrpcService.Ins.SendGameMessage(content_type, content_group, JsonUtility.ToJson(serializable_obj));
    }
    public void Callback(string json_data) {
        // after grpc and async_mq
        var result = JsonUtility.FromJson<T>(json_data);
        foreach(var callback in _callbacks){
            callback(result);
        }
    }
}

