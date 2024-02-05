using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
#if UNITY_SERVER
using Unity.Networking.Transport;
using Unity.Megacity.UI;
#endif

namespace Unity.Megacity
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    [CreateAfter(typeof(NetworkStreamReceiveSystem))]
    public partial struct ServerInGame : ISystem
    {
        private Mathematics.Random m_Random;
        private ComponentLookup<NetworkId> networkIdFromEntity;
        
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PrefabOrderComponent>();
            networkIdFromEntity = state.GetComponentLookup<NetworkId>(true);
            // var currentTime = DateTime.Now;
            // var seed = currentTime.Minute + currentTime.Second + currentTime.Millisecond + 1;
            // m_Random = new Mathematics.Random((uint)seed);
#if UNITY_SERVER
            SystemAPI.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(NetworkEndpoint.AnyIpv4.WithPort(ModeBootstrap.Options.UserSpecifiedEndpoint.Port));
#endif
        }

        public void OnUpdate(ref SystemState state)
        {
            networkIdFromEntity.Update(ref state);
            var prefab = SystemAPI.GetSingleton<PrefabOrderComponent>().value;
            var cmdBuffer = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (request, command, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<GoInGameCommand>>().WithNone<NetworkStreamInGame>()
                         .WithEntityAccess())
            {
                Debug.Log("AAA");
                cmdBuffer.AddComponent<NetworkStreamInGame>(request.ValueRO.SourceConnection);
                var player = cmdBuffer.Instantiate(prefab);
                var networkId = networkIdFromEntity[request.ValueRO.SourceConnection];
                
                cmdBuffer.SetComponent(player, new GhostOwner { NetworkId = networkId.Value });
                cmdBuffer.SetComponent(player, new MyIdComponent { value = command.ValueRO.id });
                
                // cmdBuffer.AppendToBuffer(entity, new LinkedEntityGroup { Value = player });
                // cmdBuffer.AddComponent<GhostConnectionPosition>(entity);
                // cmdBuffer.SetComponent(entity, new CommandTarget { targetEntity = player });
                
                cmdBuffer.DestroyEntity(entity);
            }

            cmdBuffer.Playback(state.EntityManager);
        }
    }
}