using System;
using System.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.Scenes;
using UnityEngine;

namespace Unity.Megacity.UI
{
    public class Conection : MonoBehaviour
    {
        [SerializeField] private string _connectIP = "127.0.0.1";
        [SerializeField] private ushort _port = 7979;
        
        [SerializeField] private bool IsEnabled = false;
        public static World clientWorld = null;

        private void Start()
        {
#if UNITY_EDITOR
            if (IsEnabled)
                StartCoroutine(Connect());
            
#endif
        }

        private IEnumerator Connect()
        {
            // if (_role == Role.ServerClient || _role == Role.Client)
            clientWorld = ClientServerBootstrap.CreateClientWorld("ClientWorld");
            
            foreach (var world in World.All)
            {
                if (world.Flags == WorldFlags.Game)
                {
                    world.Dispose();
                    break;
                }
            }
            World.DefaultGameObjectInjectionWorld = clientWorld;

            SubScene[] subScenes = FindObjectsByType<SubScene>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            
            while (!clientWorld.IsCreated)
            {
                yield return null;
            }

            if (subScenes != null)
            {
                for (int i = 0; i < subScenes.Length; i++)
                {
                    SceneSystem.LoadParameters loadParameters = new SceneSystem.LoadParameters()
                        { Flags = SceneLoadFlags.BlockOnStreamIn };
                    var sceneEntity = SceneSystem.LoadSceneAsync(clientWorld.Unmanaged,
                        new Unity.Entities.Hash128(subScenes[i].SceneGUID.Value));
                    // while (!SceneSystem.IsSceneLoaded(clientWorld.Unmanaged, sceneEntity))
                    // {
                    //     clientWorld.Update();
                    // }
                }
            }

            using var query =
                clientWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            query.GetSingletonRW<NetworkStreamDriver>().ValueRW
                .Connect(clientWorld.EntityManager, NetworkEndpoint.Parse(_connectIP, _port));
            
        }
    }
}