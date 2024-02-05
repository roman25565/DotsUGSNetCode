#if !UNITY_SERVER
using System.Threading.Tasks;
using LobbyRelaySample;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Samples.HelloNetcode
{
    /// <summary>
    /// HUD implementation. Implements behaviour for the buttons, hosting server, joining client, and starting game.
    ///
    /// Text fields output the status of server and client registering with the relay server once the user presses
    /// the respective buttons.
    ///
    /// A bootstrap world is constructed to run the jobs for setting up host and client configuration for relay server.
    /// Once this is done the game can be launched and the configuration can be retrieved from the constructed world.
    /// </summary>
    public class RelayFrontend : MonoBehaviour
    {
        string m_OldValue;
        ConnectionState m_State;
        HostServer m_HostServerSystem;
        ConnectingPlayer m_HostClientSystem;
        
        private bool isHost;
        private bool isPlayer;

        private string _relayCode;

        enum ConnectionState
        {
            Unknown,
            SetupHost,
            SetupClient,
            JoinGame,
            JoinLocalGame,
        }
        
        public void StartNetworkedGame(LocalLobby localLobby, LocalPlayer localPlayer)
        {
            isHost = localPlayer.IsHost.Value;
            isPlayer = localPlayer.IsHost.Value == false;
            Debug.Log("isHost: " + isHost);
            // m_doesNeedCleanup = true;
            // m_lobby = localLobby;
            
            CreateNetworkManager(localLobby);
        }
        
        /// <summary>
        /// The prefab with the NetworkManager contains all of the assets and logic needed to set up the NGO minigame.
        /// The UnityTransport needs to also be set up with a new Allocation from Relay.
        /// </summary>
        private async void CreateNetworkManager(LocalLobby localLobby)
        {
            if (isHost)
            {
                // StartClientServer();
                m_State = ConnectionState.SetupHost;
            }
            else if (isPlayer)
            {
                // ConnectToServer();
                await AwaitRelayCode(localLobby);
                m_State = ConnectionState.SetupClient;
            }
        }
        private async Task AwaitRelayCode(LocalLobby localLobby)
        {
            _relayCode = localLobby.RelayCode.Value;
            localLobby.RelayCode.onChanged += (code) => _relayCode = code;
            while (string.IsNullOrEmpty(_relayCode))
            {
                await Task.Delay(100);
            }
        }
        public void Update()
        {
            switch (m_State)
            {
                case ConnectionState.SetupHost:
                {
                    HostServer();
                    m_State = ConnectionState.SetupClient;
                    goto case ConnectionState.SetupClient;
                }
                case ConnectionState.SetupClient:
                {
                    var isServerHostedLocally = m_HostServerSystem?.RelayServerData.Endpoint.IsValid;
                    var enteredJoinCode = !string.IsNullOrEmpty(_relayCode);
                    if (isServerHostedLocally.GetValueOrDefault())
                    {
                        SetupClient();
                        m_HostClientSystem.GetJoinCodeFromHost();
                        m_State = ConnectionState.JoinLocalGame;
                        goto case ConnectionState.JoinLocalGame;
                    }

                    if (enteredJoinCode)
                    {
                        JoinAsClient();
                        m_State = ConnectionState.JoinGame;
                        goto case ConnectionState.JoinGame;
                    }
                    break;
                }
                case ConnectionState.JoinGame:
                {
                    var hasClientConnectedToRelayService = m_HostClientSystem?.RelayClientData.Endpoint.IsValid;
                    if (hasClientConnectedToRelayService.GetValueOrDefault())
                    {
                        Debug.Log("ConnectionState.JoinGameIf");
                        ConnectToRelayServer();
                        m_State = ConnectionState.Unknown;
                    }
                    break;
                }
                case ConnectionState.JoinLocalGame:
                {
                    var hasClientConnectedToRelayService = m_HostClientSystem?.RelayClientData.Endpoint.IsValid;
                    if (hasClientConnectedToRelayService.GetValueOrDefault())
                    {
                        SetupRelayHostedServerAndConnect();
                        m_State = ConnectionState.Unknown;
                    }
                    break;
                }
                case ConnectionState.Unknown:
                default: return;
            }
        }

        void HostServer()
        {
            var world = World.All[0];
            m_HostServerSystem = world.GetOrCreateSystemManaged<HostServer>();
            var enableRelayServerEntity = world.EntityManager.CreateEntity(ComponentType.ReadWrite<EnableRelayServer>());
            world.EntityManager.AddComponent<EnableRelayServer>(enableRelayServerEntity);

            m_HostServerSystem.UIBehaviour = this;
            var simGroup = world.GetExistingSystemManaged<SimulationSystemGroup>();
            simGroup.AddSystemToUpdateList(m_HostServerSystem);
        }

        void SetupClient()
        {
            var world = World.All[0];
            m_HostClientSystem = world.GetOrCreateSystemManaged<ConnectingPlayer>();
            m_HostClientSystem.UIBehaviour = this;
            var simGroup = world.GetExistingSystemManaged<SimulationSystemGroup>();
            simGroup.AddSystemToUpdateList(m_HostClientSystem);
        }

        void JoinAsClient()
        {
            SetupClient();
            var world = World.All[0];
            var enableRelayServerEntity = world.EntityManager.CreateEntity(ComponentType.ReadWrite<EnableRelayServer>());
            world.EntityManager.AddComponent<EnableRelayServer>(enableRelayServerEntity);
            m_HostClientSystem.JoinUsingCode(_relayCode);
        }

        /// <summary>
        /// Collect relay server end point from completed systems. Set up server with relay support and connect client
        /// to hosted server through relay server.
        /// Both client and server world is manually created to allow us to override the <see cref="DriverConstructor"/>.
        ///
        /// Two singleton entities are constructed with listen and connect requests. These will be executed asynchronously.
        /// Connecting to relay server will not be bound immediately. The Request structs will ensure that we
        /// continuously poll until the connection is established.
        /// </summary>
        void SetupRelayHostedServerAndConnect()
        {
            if (ClientServerBootstrap.RequestedPlayType != ClientServerBootstrap.PlayType.ClientAndServer)
            {
                UnityEngine.Debug.LogError($"Creating client/server worlds is not allowed if playmode is set to {ClientServerBootstrap.RequestedPlayType}");
                return;
            }

            var world = World.All[0];
            var relayClientData = world.GetExistingSystemManaged<ConnectingPlayer>().RelayClientData;
            var relayServerData = world.GetExistingSystemManaged<HostServer>().RelayServerData;
            var joinCode = world.GetExistingSystemManaged<HostServer>().JoinCode;

            var oldConstructor = NetworkStreamReceiveSystem.DriverConstructor;
            Debug.LogError("relayServerData: " + relayServerData.Endpoint);
            Debug.LogError("relayClientData: " + relayClientData.Endpoint);
            NetworkStreamReceiveSystem.DriverConstructor = new RelayDriverConstructor(relayServerData, relayClientData);
            var server = ClientServerBootstrap.CreateServerWorld("ServerWorld2");
            var client = ClientServerBootstrap.CreateClientWorld("ClientWorld");
            NetworkStreamReceiveSystem.DriverConstructor = oldConstructor;

            // SceneManager.LoadScene("FrontendHUD");
            // SceneManager.LoadScene("RelayHUD", LoadSceneMode.Additive);

            //Destroy the local simulation world to avoid the game scene to be loaded into it
            //This prevent rendering (rendering from multiple world with presentation is not greatly supported)
            //and other issues.
            DestroyLocalSimulationWorld();
            if (World.DefaultGameObjectInjectionWorld == null)
                World.DefaultGameObjectInjectionWorld = server;

            // SceneManager.LoadSceneAsync("Server", LoadSceneMode.Additive);
            SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
            SceneManager.UnloadSceneAsync("MainMenu");

            var joinCodeEntity = server.EntityManager.CreateEntity(ComponentType.ReadOnly<JoinCode>());
            server.EntityManager.SetComponentData(joinCodeEntity, new JoinCode { Value = joinCode });

            var networkStreamEntity = server.EntityManager.CreateEntity(ComponentType.ReadWrite<NetworkStreamRequestListen>());
            server.EntityManager.SetName(networkStreamEntity, "NetworkStreamRequestListen");
            server.EntityManager.SetComponentData(networkStreamEntity, new NetworkStreamRequestListen { Endpoint = NetworkEndpoint.AnyIpv4 });

            networkStreamEntity = client.EntityManager.CreateEntity(ComponentType.ReadWrite<NetworkStreamRequestConnect>());
            client.EntityManager.SetName(networkStreamEntity, "NetworkStreamRequestConnect");
            // For IPC this will not work and give an error in the transport layer. For this sample we force the client to connect through the relay service.
            // For a locally hosted server, the client would need to connect to NetworkEndpoint.AnyIpv4, and the relayClientData.Endpoint in all other cases.
            client.EntityManager.SetComponentData(networkStreamEntity, new NetworkStreamRequestConnect { Endpoint = relayClientData.Endpoint });
        }

        void ConnectToRelayServer()
        {
            var world = World.All[0];
            var relayClientData = world.GetExistingSystemManaged<ConnectingPlayer>().RelayClientData;

            var oldConstructor = NetworkStreamReceiveSystem.DriverConstructor;
            NetworkStreamReceiveSystem.DriverConstructor = new RelayDriverConstructor(new RelayServerData(), relayClientData);
            var client = ClientServerBootstrap.CreateClientWorld("ClientWorld");
            NetworkStreamReceiveSystem.DriverConstructor = oldConstructor;

            // SceneManager.LoadScene("FrontendHUD");

            //Destroy the local simulation world to avoid the game scene to be loaded into it
            //This prevent rendering (rendering from multiple world with presentation is not greatly supported)
            //and other issues.
            DestroyLocalSimulationWorld();
            if (World.DefaultGameObjectInjectionWorld == null)
                World.DefaultGameObjectInjectionWorld = client;

            SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
            SceneManager.UnloadSceneAsync("MainMenu");

            var networkStreamEntity = client.EntityManager.CreateEntity(ComponentType.ReadWrite<NetworkStreamRequestConnect>());
            client.EntityManager.SetName(networkStreamEntity, "NetworkStreamRequestConnect");
            // For IPC this will not work and give an error in the transport layer. For this sample we force the client to connect through the relay service.
            // For a locally hosted server, the client would need to connect to NetworkEndpoint.AnyIpv4, and the relayClientData.Endpoint in all other cases.
            client.EntityManager.SetComponentData(networkStreamEntity, new NetworkStreamRequestConnect { Endpoint = relayClientData.Endpoint });
        }

        #region Frontend
        
        internal static string OldFrontendWorldName = string.Empty;

        
        // Populate the scene dropdown depending on if samples/hellonetcode selection is picked in the first
        // dropdown. Always skip frontend scene since that's the one which is showing this menu and makes no sense to
        // load (as well as HUD scenes since they are additively loaded on top of sample scenes)
        
        protected void DestroyLocalSimulationWorld()
        {
            foreach (var world in World.All)
            {
                if (world.Flags == WorldFlags.Game)
                {
                    OldFrontendWorldName = world.Name;
                    world.Dispose();
                    break;
                }
            }
        }
        #endregion
    }
}
#endif
