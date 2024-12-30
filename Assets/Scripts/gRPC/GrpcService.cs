using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Grpc.Core;
using DockService;
using Mono.Cecil.Cil;
using ToolI;
using System;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class GrpcService : MonoBehaviour {
    Channel channel;
    DockService.DockService.DockServiceClient client;
    List<ChannelOption> channelOptions = new List<ChannelOption>{
        new ChannelOption("grpc.keepalive_time_ms", 10000), // Keepalive时间间隔，指定多久发送一次Keepalive探测包
        new ChannelOption("grpc.keepalive_timeout_ms", 5000), // Keepalive超时时间，指定等待Keepalive响应的时间
        new ChannelOption("grpc.keepalive_permit_without_calls", 1), // 如果在等待Keepalive响应时连接丢失，是否关闭连接
    };
    Dictionary<string, CancellationTokenSource> cancelTokenSourceDict = new Dictionary<string, CancellationTokenSource>{
        {"GameStream",null},
        {"GameStreamNew",null},
        {"MatchStream",null},
    };
    // string server_url = "https://global.huoban.ai:38310";
    // string server_url = "117.134.14.85:38310";
    [SerializeField]
    GrpcVersion version = new GrpcVersion();
    string server_url = "129.211.6.217:38311";
    private static GrpcService _instance;
    public static GrpcService Ins {
        get {
            return _instance;
        }
    }
    // user_info
    public UserInfo user_info;
    public string user_id {
        get {
            return user_info.user_id;
        }
    }

    void Awake() {
        _instance = this;
        DontDestroyOnLoad(gameObject);
        channel = new Channel(server_url, ChannelCredentials.Insecure, channelOptions);
        client = new DockService.DockService.DockServiceClient(channel);
    }
    void OnApplicationQuit() {
        if (cancelTokenSourceDict["MatchStream"] != null) {
            CancelMatch();
        }
        QuitAllStream();
        Debug.Log("gRPC shutdown");
        channel.ShutdownAsync().Wait();
    }
    public void QuitAllStream() {
        foreach (var pair in cancelTokenSourceDict) {
            if (pair.Value != null) {
                pair.Value.Cancel();
            }
        }
    }
    public void QuitGameStream() {
        if (cancelTokenSourceDict["GameStream"] != null) {
            cancelTokenSourceDict["GameStream"].Cancel();
        }
        if (cancelTokenSourceDict["GameStreamNew"] != null) {
            cancelTokenSourceDict["GameStreamNew"].Cancel();
        }
    }




    public (bool is_ok, GrpcVersion cur_version, GrpcVersion new_version) Connect() {
        try {
            var message = new ConnectRequest();
            var response = client.Connect(message);
            Debug.Log(response.Content);
            var content = JsonUtility.FromJson<GrpcConnectContent>(response.Content);
            var result = version.Validate(content);
            return (response.IsOk && result, version, new GrpcVersion(content));
        }
        catch {
            return (false, version, new GrpcVersion());
        }
    }
    public (bool is_success, string user_id, string content) Login(string account, string password) {
        var message = new LoginRequest {
            Account = account,
            Password = password,
        };
        var response = client.LoginAccount(message);
        Debug.Log(response);
        return (response.IsSuccess, response.UserId, response.Content);
    }
    public (bool is_success, string content) Register(string account, string password) {
        var message = new RegisterRequest {
            Account = account,
            Password = password,
        };
        var response = client.RegisterAccount(message);
        Debug.Log(response);
        return (response.IsSuccess, response.Content);
    }
    public delegate void GetUserInfoCallback(string content);
    public void GetUserInfo(GetUserInfoCallback callback = null) {
        var message = new GetUserInfoRequest {
            UserId = user_id,
        };
        var response = client.GetUserInfo(message);
        Debug.Log(response);
        callback?.Invoke(response.Content);
    }
    public delegate void MatchCallback(bool is_success, string content);
    public async void Match(MatchCallback callback = null) {
        var message = new MatchRequest {
            UserId = user_id,
        };
        var cancellationTokenSource = new CancellationTokenSource();
        cancelTokenSourceDict["MatchStream"] = cancellationTokenSource;
        var response_iter = client.MatchGameRoom(message, cancellationToken: cancellationTokenSource.Token);
        try {
            while (await response_iter.ResponseStream.MoveNext(cancellationTokenSource.Token)) {
                var response = response_iter.ResponseStream.Current;
                Debug.Log(response);
                callback?.Invoke(response.IsSuccess, response.Content);
            }
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.Cancelled) {
            Debug.Log("gRPC MatchStream was cancelled.");
        }
        catch (OperationCanceledException) {
            Debug.Log("Match Operation was cancelled.");
        }
        Debug.Log("all match over");
    }
    public delegate void CancelMatchCallback(bool is_success, string content);
    public async void CancelMatch(CancelMatchCallback callback = null) {
        var message = new MatchRequest {
            UserId = user_id,
        };
        var response = await client.CancelMatchingAsync(message);
        Debug.Log(response);
        callback?.Invoke(response.IsSuccess, response.Content);
    }

    // ------- Test --------

    private AsyncQueue<GameMessage> send_message_buffer = new AsyncQueue<GameMessage>();
    public void SendGameMessage(string content_type, string content_group, string content = "") {
        var message = new GameMessage {
            UserId = user_id,
            CreateAt = TimeI.GetTimeStr(),
            ContentType = content_type,
            ContentGroup = content_group,
            Content = content,
        };
        send_message_buffer.Enqueue(message);
    }
    public async void GameMessageStream(UnityAction<string, string, string> callback = null) {
        try {
            var header = new Metadata {
                { "user_id", user_id }
            };
            var cts = new CancellationTokenSource();
            cancelTokenSourceDict["GameStreamNew"] = cts;
            var call = client.GameMessageStream(header, cancellationToken: cts.Token);

            Debug.Log("GameMessageStream Successfully Started");
            var writeTask = Task.Run(async () => {
                while (cts.IsCancellationRequested == false) {
                    var request = await send_message_buffer.Dequeue();
                    await call.RequestStream.WriteAsync(request);
                    Debug.Log("GameMessageStream[Send]: " + request);
                };
            }, cts.Token);
            Debug.Log("readTask Successfully Started");

            while (await call.ResponseStream.MoveNext(cts.Token)) {
                var response = call.ResponseStream.Current;
                Debug.Log("GameMessageStream[Reiceive]: " + response);
                if (callback != null) {
                    callback(response.ContentType, response.ContentGroup, response.Content);
                }
            }

            Debug.Log("GameMessageStream Stopped");
            await call.RequestStream.CompleteAsync();
            await writeTask;
        }
        catch (RpcException e) when (e.StatusCode == StatusCode.Cancelled) {
            Debug.Log("gRPC GameStream was cancelled.");
        }
        catch (OperationCanceledException) {
            Debug.Log("GameStream Operation was cancelled.");
        }
    }
}
