// using Grpc.Core;
// using DockTest;
// using System.Drawing.Printing;
// using System.Diagnostics;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class cs : MonoBehaviour
// {
//     MyClient client;
//     // Start is called before the first frame update
//     void Start()
//     {
//         client = new MyClient();
//         client.CallMyMethod("hello");
//     }
// }

// class MyClient
// {
//     private Channel channel;
//     private MyService.MyServiceClient client;

//     public MyClient()
//     {
//         channel = new Channel("localhost:50051", ChannelCredentials.Insecure);
//         client = new MyService.MyServiceClient(channel);
//     }

//     public string CallMyMethod(string message)
//     {
//         var request = new MyRequest { Message = message };
//         var response = client.MyMethod(request);
//         UnityEngine.Debug.Log(response.Message);
//         return response.Message;
//     }

//     public void Shutdown()
//     {
//         channel.ShutdownAsync().Wait();
//     }
// }
//FIXME
//DEBUG
//TODO: 12387
//REVIEW,123435154
//IDEA
//todo