using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LitJson;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using ToolI;

public class EventManager : BaseBehaviour {
    private static EventManager _instance;
    public static EventManager Ins {
        get {
            return _instance;
        }
    }
    public Dictionary<string, IGrpcCallback> grpc_event_dict = new Dictionary<string, IGrpcCallback>();
    public Dictionary<string, AsyncQueue<string>> ordered_message_queue = new Dictionary<string, AsyncQueue<string>>();
    void Awake() {
        _instance = this;

        refresh_shop = new UnityEvent();
        round_change = new UnityEvent();
        coin_change = new UnityEvent();
        bean_change = new UnityEvent();
        exp_change = new UnityEvent();
        round_time_change = new UnityEvent();

        actor_mil_change = new UnityEvent<XActor>();
        actors_change = new UnityEvent();
    }

    public UnityEvent grpc_event_state_change = new UnityEvent();
    public UnityEvent<bool> round_switching = new UnityEvent<bool>();
    public UnityEvent refresh_shop = new UnityEvent();
    public UnityEvent round_change = new UnityEvent();
    public UnityEvent coin_change = new UnityEvent();
    public UnityEvent bean_change = new UnityEvent();
    public UnityEvent exp_change = new UnityEvent();
    public UnityEvent round_time_change = new UnityEvent();
    public UnityEvent act_time_change = new UnityEvent();

    // ----- Actor -----
    public UnityEvent actors_change;
    public UnityEvent<XActor> actor_mil_change = new UnityEvent<XActor>();
    // public UnityEvent<XChess> chess_after_act = new UnityEvent<XChess>();

}

