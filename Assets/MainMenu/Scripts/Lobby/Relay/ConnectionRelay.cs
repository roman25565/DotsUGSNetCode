using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace MainMenu.ngo
{
    /// <summary>
    /// Once the local localPlayer is in a localLobby and that localLobby has entered the In-Game state, this will load in whatever is necessary to actually run the game part.
    /// This will exist in the game scene so that it can hold references to scene objects that spawned prefab instances will need.
    /// </summary>
    public class ConnectionRelay : MonoBehaviour
    {
    //     private bool m_doesNeedCleanup = false;
    //     private bool m_hasConnectedViaNGO = false;
    //
    //     private LocalLobby m_lobby;
    //
    //     private string hostLatestMessageReceived;
    //     private string playerLatestMessageReceived;
    //     
    //     private Allocation hostAllocation;
    //     private JoinAllocation playerAllocation;
    //     
    //     private List<Region> regions = new List<Region>();
    //     private List<string> regionOptions = new List<string>();
    //     
    //     private bool isHost;
    //     private bool isPlayer;
    //     
    //     private NetworkDriver hostDriver;
    //     private NetworkDriver playerDriver;
    //     private NativeList<NetworkConnection> serverConnections;
    //     private NetworkConnection clientConnection;
    //
    //     private RelayServerData _relayServerData;
    //     private RelayServerData _relayClientData;
    //
    //     private void Update()
    //     {
    //         if (isHost)
    //         {
    //             UpdateHost();
    //         }
    //         else if (isPlayer)
    //         {
    //             UpdatePlayer();
    //         }
    //     }
    //
    //     public async Task StartNetworkedGame(LocalLobby localLobby, LocalPlayer localPlayer)
    //     {
    //         isHost = localPlayer.IsHost.Value;
    //         isPlayer = localPlayer.IsHost.Value == false;
    //         Debug.Log("isHost: " + isHost);
    //         m_doesNeedCleanup = true;
    //         m_lobby = localLobby;
    //         
    //         await CreateNetworkManager();
    //     }
    //     
    //     /// <summary>
    //     /// The prefab with the NetworkManager contains all of the assets and logic needed to set up the NGO minigame.
    //     /// The UnityTransport needs to also be set up with a new Allocation from Relay.
    //     /// </summary>
    //     async Task CreateNetworkManager()
    //     {
    //         if (isHost)
    //         {
    //             await CreatingRelayHostAllocation();
    //             var relayServerData = BindHostToRelayUTP(out _relayServerData);
    //             var joinCode = await GetJoinCodeRelay();
    //             MainMenuManager.Instance.HostSetRelayCode(joinCode);
    //             await Task.Delay(1000);
    //             Debug.Log("isHost(): start");
    //             ParseNetCodeForEntities(relayServerData);
    //             SetupRelayHostedServerAndConnect();
    //         }
    //         else if (isPlayer)
    //         {
    //             await AwaitRelayCode();
    //             await JoinRelay();
    //             var relayServerData = BindPlayerToRelayUTP(out _relayClientData);
    //             ConnectPlayerToHostUTP();
    //             Debug.Log("isPlayer(): start");
    //             await Task.Delay(1000);
    //             OnPlayerSendMessage();
    //             ParseNetCodeForEntities(relayServerData);
    //         }
    //     }
    //
    //     async Task AwaitRelayCode()
    //     {
    //         string relayCode = m_lobby.RelayCode.Value;
    //         m_lobby.RelayCode.onChanged += (code) => relayCode = code;
    //         while (string.IsNullOrEmpty(relayCode))
    //         {
    //             await Task.Delay(100);
    //         }
    //     }
    //
    //     async Task CreatingRelayHostAllocation()
    //     {
    //         Debug.Log("Host - Creating an allocation. Upon success, I have 10 seconds to BIND to the Relay server that I've allocated.");
    //
    //         // Determine region to use (user-selected or auto-select/QoS)
    //         string region = GetRegionOrQosDefault();
    //         Debug.Log($"The chosen region is: {region}");
    //
    //         // Set max connections. Can be up to 100, but note the more players connected, the higher the bandwidth/latency impact.
    //         int maxConnections = m_lobby.MaxPlayerCount.Value;
    //
    //         // Important: Once the allocation is created, you have ten seconds to BIND, else the allocation times out.
    //         hostAllocation = await RelayService.Instance.CreateAllocationAsync(maxConnections, region);
    //         Debug.Log($"Host Allocation ID: {hostAllocation.AllocationId}, region: {hostAllocation.Region}");
    //
    //         // Initialize NetworkConnection list for the server (Host).
    //         // This list object manages the NetworkConnections which represent connected players.
    //         serverConnections = new NativeList<NetworkConnection>(maxConnections, Allocator.Persistent);
    //     }
    //
    //     async Task JoinRelay()
    //     {
    //         try
    //         {
    //             playerAllocation = await RelayService.Instance.JoinAllocationAsync(m_lobby.RelayCode.Value);
    //             Debug.Log("Player Allocation ID: " + playerAllocation.AllocationId);
    //         }
    //         catch (RelayServiceException ex)
    //         {
    //             Debug.LogError(ex.Message + "\n" + ex.StackTrace);
    //         }
    //     }
    //
    //     private RelayServerEndpoint BindPlayerToRelayUTP(out RelayServerData relayServerData)
    //     {
    //         Debug.Log("Player - Binding to the Relay server using UTP.");
    //
    //         // Extract the Relay server data from the Join Allocation response.
    //         relayServerData = new RelayServerData(playerAllocation, "udp");
    //         
    //         // relayServerData.Endpoint.Address
    //         // Create NetworkSettings using the Relay server data.
    //         var settings = new NetworkSettings();
    //         settings.WithRelayParameters(ref relayServerData);
    //
    //         // Create the Player's NetworkDriver from the NetworkSettings object.
    //         playerDriver = NetworkDriver.Create(settings);
    //
    //         // Bind to the Relay server.
    //         if (playerDriver.Bind(NetworkEndpoint.AnyIpv4) != 0)
    //         {
    //             Debug.LogError("Player client failed to bind");
    //         }
    //         else
    //         {
    //             Debug.Log("Player client bound to Relay server");
    //         }
    //
    //         var aRelayServerEndpoint = GetEndpointForConnectionType(playerAllocation.ServerEndpoints, "udp");
    //         return aRelayServerEndpoint;
    //     }
    //     
    //     public void ConnectPlayerToHostUTP()
    //     {
    //         Debug.Log("Player - Connecting to Host's client.");
    //
    //         // Sends a connection request to the Host Player.
    //         clientConnection = playerDriver.Connect();
    //     }
    //     
    //     public void OnHostSendMessage()
    //     {
    //         if (serverConnections.Length == 0)
    //         {
    //             Debug.LogError("No players connected to send messages to.");
    //             return;
    //         }
    //
    //         var msg = "Hello World!";
    //
    //         // In this sample, we will simply broadcast a message to all connected clients.
    //         for (int i = 0; i < serverConnections.Length; i++)
    //         {
    //             if (hostDriver.BeginSend(serverConnections[i], out var writer) == 0)
    //             {
    //                 // Send the message. Aside from FixedString32, many different types can be used.
    //                 writer.WriteFixedString32(msg);
    //                 hostDriver.EndSend(writer);
    //             }
    //         }
    //     }
    //     private void OnPlayerSendMessage()
    //     {
    //         Debug.Log("OnPlayerSendMessage(): start");
    //         if (!clientConnection.IsCreated)
    //         {
    //             Debug.LogError("Player is not connected. No Host client to send message to.");
    //             return;
    //         }
    //         var msg = "Hello World!";
    //         if (playerDriver.BeginSend(clientConnection, out var writer) == 0)
    //         {
    //             // Send the message. Aside from FixedString32, many different types can be used.
    //             writer.WriteFixedString32(msg);
    //             playerDriver.EndSend(writer);
    //         }
    //     }
    //
    //     string GetRegionOrQosDefault()
    //     {
    //         // Return null (indicating to auto-select the region/QoS) if regions list is empty OR auto-select/QoS is chosen
    //         // if (!regions.Any() || RegionsDropdown.value == regionAutoSelectIndex)
    //         // {
    //             return null;
    //         // }
    //         // else use chosen region (offset -1 in dropdown due to first option being auto-select/QoS)
    //         // return regions[RegionsDropdown.value - 1].Id;
    //     }
    //     private RelayServerEndpoint BindHostToRelayUTP(out RelayServerData relayServerData)
    //     {
    //         Debug.Log("Host - Binding to the Relay server using UTP.");
    //
    //         // Extract the Relay server data from the Allocation response.
    //         relayServerData = new RelayServerData(hostAllocation, "udp");
    //         Debug.Log("[Relay]Endpoint.Address" + relayServerData.Endpoint.Address);
    //         Debug.Log("[Relay]Endpoint.Port" + relayServerData.Endpoint.Port);
    //         // Create NetworkSettings using the Relay server data.
    //         var settings = new NetworkSettings();
    //         settings.WithRelayParameters(ref relayServerData);
    //
    //         // Create the Host's NetworkDriver from the NetworkSettings.
    //         hostDriver = NetworkDriver.Create(settings);
    //
    //         // Bind to the Relay server.
    //         if (hostDriver.Bind(NetworkEndpoint.AnyIpv4) != 0)
    //         {
    //             Debug.LogError("Host client failed to bind");
    //         }
    //         else
    //         {
    //             if (hostDriver.Listen() != 0)
    //             {
    //                 Debug.LogError("Host client failed to listen");
    //             }
    //             else
    //             {
    //                 Debug.Log("Host client bound to Relay server");
    //             }
    //         }
    //         var aRelayServerEndpoint = GetEndpointForConnectionType(hostAllocation.ServerEndpoints, "udp");
    //         return aRelayServerEndpoint;
    //     }
    //
    //     private async Task<string> GetJoinCodeRelay()
    //     {
    //         var joinCode = "";
    //         try
    //         {
    //             joinCode = await RelayService.Instance.GetJoinCodeAsync(hostAllocation.AllocationId);
    //             Debug.Log("Host - Got join code: " + joinCode); 
    //         }
    //         catch (RelayServiceException ex)
    //         {
    //             Debug.LogError(ex.Message + "\n" + ex.StackTrace);
    //         }
    //         return joinCode;
    //     }
    //     
    //     void OnDestroy()
    //     {
    //         // Cleanup objects upon exit
    //         if (isHost)
    //         {
    //             hostDriver.Dispose();
    //             serverConnections.Dispose();
    //         }
    //         else if(isPlayer)
    //         {
    //             playerDriver.Dispose();
    //         }
    //     }
    //     void UpdateHost()
    //     {
    //         // Skip update logic if the Host is not yet bound.
    //         if (!hostDriver.IsCreated || !hostDriver.Bound)
    //         {
    //             return;
    //         }
    //
    //         // This keeps the binding to the Relay server alive,
    //         // preventing it from timing out due to inactivity.
    //         hostDriver.ScheduleUpdate().Complete();
    //
    //         // Clean up stale connections.
    //         for (int i = 0; i < serverConnections.Length; i++)
    //         {
    //             if (!serverConnections[i].IsCreated)
    //             {
    //                 Debug.Log("Stale connection removed");
    //                 serverConnections.RemoveAt(i);
    //                 --i;
    //             }
    //         }
    //
    //         // Accept incoming client connections.
    //         NetworkConnection incomingConnection;
    //         while ((incomingConnection = hostDriver.Accept()) != default(NetworkConnection))
    //         {
    //             // Adds the requesting Player to the serverConnections list.
    //             // This also sends a Connect event back the requesting Player,
    //             // as a means of acknowledging acceptance.
    //             Debug.Log("Accepted an incoming connection.");
    //             serverConnections.Add(incomingConnection);
    //         }
    //
    //         // Process events from all connections.
    //         for (int i = 0; i < serverConnections.Length; i++)
    //         {
    //             Assert.IsTrue(serverConnections[i].IsCreated);
    //
    //             // Resolve event queue.
    //             NetworkEvent.Type eventType;
    //             while ((eventType = hostDriver.PopEventForConnection(serverConnections[i], out var stream)) != NetworkEvent.Type.Empty)
    //             {
    //                 switch (eventType)
    //                 {
    //                     // Handle Relay events.
    //                     case NetworkEvent.Type.Data:
    //                         FixedString32Bytes msg = stream.ReadFixedString32();
    //                         Debug.Log($"Server received msg: {msg}");
    //                         hostLatestMessageReceived = msg.ToString();
    //                         break;
    //
    //                     // Handle Disconnect events.
    //                     case NetworkEvent.Type.Disconnect:
    //                         Debug.Log("Server received disconnect from client");
    //                         serverConnections[i] = default(NetworkConnection);
    //                         break;
    //                 }
    //             }
    //         }
    //     }
    //
    // void UpdatePlayer()
    // {
    //     // Skip update logic if the Player is not yet bound.
    //     if (!playerDriver.IsCreated || !playerDriver.Bound)
    //     {
    //         return;
    //     }
    //
    //     // This keeps the binding to the Relay server alive,
    //     // preventing it from timing out due to inactivity.
    //     playerDriver.ScheduleUpdate().Complete();
    //
    //     // Resolve event queue. 
    //     NetworkEvent.Type eventType;
    //     while ((eventType = clientConnection.PopEvent(playerDriver, out var stream)) != NetworkEvent.Type.Empty)
    //     {
    //         switch (eventType)
    //         {
    //             // Handle Relay events.
    //             case NetworkEvent.Type.Data:
    //                 FixedString32Bytes msg = stream.ReadFixedString32();
    //                 Debug.Log($"Player received msg: {msg}");
    //                 playerLatestMessageReceived = msg.ToString();
    //                 break;
    //
    //             // Handle Connect events.
    //             case NetworkEvent.Type.Connect:
    //                 Debug.Log("Player connected to the Host");
    //                 break;
    //
    //             // Handle Disconnect events.
    //             case NetworkEvent.Type.Disconnect:
    //                 Debug.Log("Player got disconnected from the Host");
    //                 clientConnection = default(NetworkConnection);
    //                 break;
    //         }
    //     }
    // } 
    // private void ParseNetCodeForEntities(RelayServerEndpoint relayServerEndpoint)
    // {
    //     Debug.Log("relayServerEndpoint.Host: " + relayServerEndpoint.Host);
    //     NetworkEndpoint.Parse(relayServerEndpoint.Host, (ushort)relayServerEndpoint.Port);
    // }
    // private static RelayServerEndpoint GetEndpointForConnectionType(List<RelayServerEndpoint> endpoints, string connectionType)
    // {
    //     return endpoints.FirstOrDefault(endpoint => endpoint.ConnectionType == connectionType);
    // }
    //
    // private void SetupRelayHostedServerAndConnect()
    // {
    //     if (ClientServerBootstrap.RequestedPlayType != ClientServerBootstrap.PlayType.ClientAndServer)
    //     {
    //         Debug.LogError($"Creating client/server worlds is not allowed if playmode is set to {ClientServerBootstrap.RequestedPlayType}");
    //         return;
    //     }
    //     var oldConstructor = NetworkStreamReceiveSystem.DriverConstructor;
    //     _relayClientData = default;
    //     Debug.LogError("relayServerData: " + _relayServerData.Endpoint);
    //     Debug.LogError("relayClientData: " + _relayClientData.Endpoint);
    //     NetworkStreamReceiveSystem.DriverConstructor = new RelayDriverConstructor(_relayServerData, _relayClientData);
    //     var server = ClientServerBootstrap.CreateServerWorld("ServerWorld");
    //     var client = ClientServerBootstrap.CreateClientWorld("ClientWorld");
    //     NetworkStreamReceiveSystem.DriverConstructor = oldConstructor;
    //     DestroyLocalSimulationWorld();
    //     if (World.DefaultGameObjectInjectionWorld == null)
    //         World.DefaultGameObjectInjectionWorld = server;
    //     
    //     SceneManager.UnloadSceneAsync("MainMenu");
    //     SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
    //     
    //     var networkStreamEntity = server.EntityManager.CreateEntity(ComponentType.ReadWrite<NetworkStreamRequestListen>());
    //     server.EntityManager.SetName(networkStreamEntity, "NetworkStreamRequestListen");
    //     server.EntityManager.SetComponentData(networkStreamEntity, new NetworkStreamRequestListen { Endpoint = NetworkEndpoint.AnyIpv4 });
    //
    //     Debug.LogError($"Creating client/server worlds is not allowed if playmode is set to {ClientServerBootstrap.RequestedPlayType}");
    //     return;
    //     networkStreamEntity = client.EntityManager.CreateEntity(ComponentType.ReadWrite<NetworkStreamRequestConnect>());
    //     client.EntityManager.SetName(networkStreamEntity, "NetworkStreamRequestConnect");
    //     // For IPC this will not work and give an error in the transport layer. For this sample we force the client to connect through the relay service.
    //     // For a locally hosted server, the client would need to connect to NetworkEndpoint.AnyIpv4, and the relayClientData.Endpoint in all other cases.
    //     client.EntityManager.SetComponentData(networkStreamEntity, new NetworkStreamRequestConnect { Endpoint = _relayClientData.Endpoint });
    // }
    //
    // private void DestroyLocalSimulationWorld()
    // {
    //     foreach (var world in World.All)
    //     {
    //         if (world.Flags == WorldFlags.Game)
    //         {
    //             world.Dispose();
    //             break;
    //         }
    //     }
    // }
    }
}