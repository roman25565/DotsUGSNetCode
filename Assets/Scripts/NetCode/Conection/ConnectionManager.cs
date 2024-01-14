using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectionManager : MonoBehaviour
{

    public string Address;
    public ushort Port;
    public string Scene;
   

    // Start is called before the first frame update
    private void Awake()
    {
        if (GameDataManager.Instance.ImHost)
        {
            StartHost();
        }else
        {
            StartClient();
        }
    }

    private void StartHost()
    {       
        var server = ClientServerBootstrap.CreateServerWorld("ServerWorld");
        var client = ClientServerBootstrap.CreateClientWorld("ClientWorld");
                
        DestroyLocalWorld();
        World.DefaultGameObjectInjectionWorld ??= server;

        {
            using var drvQuery = server.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            drvQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(ClientServerBootstrap.DefaultListenAddress.WithPort(Port));

            //var listenRequest = server.EntityManager.CreateEntity(typeof(NetworkStreamRequestListen));
            //server.EntityManager.SetComponentData(listenRequest, new NetworkStreamRequestListen { Endpoint = ClientServerBootstrap.DefaultListenAddress.WithPort(Port) });
        }
       
        {
            using var drvQuery = client.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            drvQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(client.EntityManager, ClientServerBootstrap.DefaultConnectAddress.WithPort(Port));

            //var connectRequest = client.EntityManager.CreateEntity(typeof(NetworkStreamRequestConnect));
            //client.EntityManager.SetComponentData(connectRequest, new NetworkStreamRequestConnect { Endpoint = ClientServerBootstrap.DefaultConnectAddress.WithPort(Port) });
        }

        
    }

    private void StartClient()
    {
        var client = ClientServerBootstrap.CreateClientWorld("ClientWorld");

        DestroyLocalWorld();
        World.DefaultGameObjectInjectionWorld ??= client;
        
        using var drvQuery = client.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
        drvQuery.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(client.EntityManager, NetworkEndpoint.Parse(Address,Port));
    }

    private void DestroyLocalWorld()
    {
        foreach (var world in World.All)
        {
            if(world.Flags == WorldFlags.Game)
            {
                world.Dispose();
                break;
            }
        }
    }
}
