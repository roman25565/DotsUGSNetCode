using Unity.Megacity;
using Unity.Megacity.UI;
using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.Services.Samples.GameServerHosting;
using UnityEngine;
using UnityEngine.SceneManagement;

[UnityEngine.Scripting.Preserve]
public class NetCodeBootstrap : ClientServerBootstrap
{
    const string k_ServicesDataResourceLocation = "GameServiceData";
    
    public static NetworkEndpoint MegacityServerIp => NetworkEndpoint.Parse("128.14.159.58", 7979);
    public const int MaxPlayerCount = 200;
    
    
    public override bool Initialize(string defaultWorldName)
    {
        NetworkStreamReceiveSystem.DriverConstructor = new DriverConstructor();
        
#if UNITY_SERVER
            // var isFrontEnd = SceneController.IsFrontEnd; 
            // if(isFrontEnd)
            // {
                // CoreBooter.instance.LoadMap("GameScene");
            // }
            SceneManager.LoadSceneAsync("Server");
            Debug.Log("YEs");
            Application.targetFrameRate = 60; 
            CreateDefaultClientServerWorlds();
            TryStartUgs();

            // Niki.Walker: Disabled as UNITY_SERVER does not support creating thin client worlds.
            // // On the server, also create thin clients (if requested).
            // TryCreateThinClientsIfRequested();

            return true;
#endif
        // CoreBooter.instance.LoadMenu();
        AsyncOperation op = SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
        //Use 0 to manually connect
        // We always set the DefaultConnectAddress in a player, because it's unlikely you'll want to test locally here.
        DefaultConnectAddress = ModeBootstrap.Options.UserSpecifiedEndpoint;
        
        ServerConnectionUtils.CreateDefaultWorld();
        return true;
    }
    
    private async void TryStartUgs()
    {
        var gameInfo = Resources.Load<GameServerInfo_Data>(k_ServicesDataResourceLocation);
        if (!gameInfo)
        {
            Debug.LogError($"[GSH] No Game Server Info Object at 'Assets/Resources/{k_ServicesDataResourceLocation}'");
            return;
        }
    
        var multiplayGameServerManager = new GameObject("MultiplayServer").AddComponent<GameServerManager>();
        multiplayGameServerManager.Init(gameInfo);
        Debug.LogError($"[GSH] Multiplay GameServer Manager {gameInfo.Data}'");
            
        if (!await multiplayGameServerManager.InitServices())
            return;
        Debug.LogError($"[GSH] Try Start Server {ModeBootstrap.Options.UserSpecifiedEndpoint.Address}");
        await multiplayGameServerManager.TryStartServer(ModeBootstrap.Options.UserSpecifiedEndpoint.Address);
    }
}
