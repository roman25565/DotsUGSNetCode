using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

public struct GoInGameCommand : IRpcCommand
{
    public int id;
}

namespace Unity.Megacity
{
    
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct ClientInGame : ISystem
    {
        private bool m_HasRegisteredSmoothingAction;

        public void OnCreate(ref SystemState state)
        {
            // var tickRate = NetworkTimeSystem.DefaultClientTickRate;
            // tickRate.MaxExtrapolationTimeSimTicks = 120;
            // tickRate.InterpolationTimeMS = 150;
            // state.EntityManager.CreateSingleton(tickRate);
            //
            // // Niki.Walker: Client-side optimizations:
            // var ghostSendSystemData = new GhostSendSystemData
            // {
            //     MinSendImportance = 2
            // };
            //
            // // Don't frequently resend the same bot vehicles.
            // //ghostSendSystemData.FirstSendImportanceMultiplier = 100; // Significantly bias towards sending new ghosts.
            // // Disabled as it ruins the start of the game.
            // state.EntityManager.CreateSingleton(ghostSendSystemData);
        }

        public void OnUpdate(ref SystemState state)
        {
            var cmdBuffer = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (netId, entity) in SystemAPI.Query<RefRO<NetworkId>>().WithNone<NetworkStreamInGame>()
                         .WithEntityAccess())
            {
                cmdBuffer.AddComponent<NetworkStreamInGame>(entity);
                
                var request = cmdBuffer.CreateEntity();
                cmdBuffer.AddComponent<GoInGameCommand>(request);
                cmdBuffer.SetComponent(request, new GoInGameCommand{id = GameDataManager.MyId });//CoreDataHandler.instance.MyId
                cmdBuffer.AddComponent<SendRpcCommandRequest>(request);
            }

            cmdBuffer.Playback(state.EntityManager);
        }
    }
}