// using System;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.SceneManagement;
// using Unity.NetCode;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Networking.Transport;
// using UnityEngine.Serialization;
//
// public enum NETLoadType
// {
//     HOST,
//     LOGIN,
// }
// public class Frontend : MonoBehaviour
// {
//     const ushort k_NetworkPort = 7979;
//
//     public NETLoadType type;
//
//         /// <summary>
//         /// Stores the old name of the local world (create by initial bootstrap).
//         /// It is reused later when the local world is created when coming back from game to the menu.
//         /// </summary>
//     internal static string OldFrontendWorldName = string.Empty; 
//        
//
//         // public void Start()
//         // {
//         //     if (type == NETLoadType.HOST)
//         //     {
//         //         StartClientServer("GameScene");
//         //         Debug.Log("HOST");
//         //     }
//         //     else if (type == NETLoadType.LOGIN)
//         //     {
//         //         ConnectToServer("GameScene");
//         //     }
//         // }
//
//         public void StartClientServer(string sceneName)
//         {
//             if (ClientServerBootstrap.RequestedPlayType != ClientServerBootstrap.PlayType.ClientAndServer)
//             {
//                 Debug.LogError($"Creating client/server worlds is not allowed if playmode is set to {ClientServerBootstrap.RequestedPlayType}");
//                 return;
//             }
//
//             var server = ClientServerBootstrap.CreateServerWorld("ServerWorld");
//             var client = ClientServerBootstrap.CreateClientWorld("ClientWorld");
//
//             //Destroy the local simulation world to avoid the game scene to be loaded into it
//             //This prevent rendering (rendering from multiple world with presentation is not greatly supported)
//             //and other issues.
//             DestroyLocalSimulationWorld();
//             if (World.DefaultGameObjectInjectionWorld == null)
//                 World.DefaultGameObjectInjectionWorld = server;
//             SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
//
//
//             NetworkEndpoint ep = NetworkEndpoint.AnyIpv4.WithPort(k_NetworkPort);
//             {
//                 using var drvQuery = server.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
//                 drvQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(ep);
//             }
//
//             ep = NetworkEndpoint.LoopbackIpv4.WithPort(k_NetworkPort);
//             {
//                 using var drvQuery = client.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
//                 drvQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(client.EntityManager, ep);
//             }
//         }
//
//         public void ConnectToServer(string sceneName)
//         {
//             var client = ClientServerBootstrap.CreateClientWorld("ClientsWorld");
//             DestroyLocalSimulationWorld();
//             ;
//             if (World.DefaultGameObjectInjectionWorld == null)
//                 World.DefaultGameObjectInjectionWorld = client;
//             SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
//
//             var ep = NetworkEndpoint.Parse("127.0.0.1", k_NetworkPort);
//             {
//                 using var drvQuery = client.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
//                 drvQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(client.EntityManager, ep);
//             }
//         }
//
//         // Populate the scene dropdown depending on if samples/hellonetcode selection is picked in the first
//         // dropdown. Always skip frontend scene since that's the one which is showing this menu and makes no sense to
//         // load (as well as HUD scenes since they are additively loaded on top of sample scenes)
//         
//
//         protected void DestroyLocalSimulationWorld()
//         {
//             foreach (var world in World.All)
//             {
//                 if (world.Flags == WorldFlags.Game)
//                 {
//                     OldFrontendWorldName = world.Name;
//                     world.Dispose();
//                     break;
//                 }
//             }
//         }
//
//         // Tries to parse a port, returns true if successful, otherwise false
//         // The port will be set to whatever is parsed, otherwise the default port of k_NetworkPort
//         private UInt16 ParsePortOrDefault(string s)
//         {
//             if (!UInt16.TryParse(s, out var port))
//             {
//                 Debug.LogWarning($"Unable to parse port, using default port {k_NetworkPort}");
//                 return k_NetworkPort;
//             }
//
//             return port;
//         }
//     }
//
