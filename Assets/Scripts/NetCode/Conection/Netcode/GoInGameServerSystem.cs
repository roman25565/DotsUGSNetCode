using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct GoInGameServerSystem : ISystem
{
    private ComponentLookup<NetworkId> networkIdFromEntity;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        var builder = new EntityQueryBuilder(Allocator.Temp);
        builder.WithAll<ReceiveRpcCommandRequest, GoInGameCommand>();
        state.RequireForUpdate(state.GetEntityQuery(builder));
        networkIdFromEntity = state.GetComponentLookup<NetworkId>(true);
        state.RequireForUpdate<PrefabOrderComponent>();
    }

    public void OnUpdate(ref SystemState state)
    {
        networkIdFromEntity.Update(ref state);
        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        foreach (var (request, command, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<GoInGameCommand>>().WithEntityAccess())
        {
            commandBuffer.AddComponent<NetworkStreamInGame>(request.ValueRO.SourceConnection);
            commandBuffer.DestroyEntity(entity);
            
            PrefabOrderComponent prefabs;
            prefabs = SystemAPI.GetSingleton<PrefabOrderComponent>();
            
            Entity player = commandBuffer.Instantiate(prefabs.value);
            commandBuffer.SetComponent(player, new MyIdComponent
            {
                value = MyId.Value
            });

            var networkId = networkIdFromEntity[request.ValueRO.SourceConnection];
            commandBuffer.SetComponent(player, new GhostOwner()
            {
                NetworkId = networkId.Value
            });

            commandBuffer.AppendToBuffer(request.ValueRO.SourceConnection, new LinkedEntityGroup()
            {
                Value = player
            });

            
            commandBuffer.DestroyEntity(entity);
        }
        commandBuffer.Playback(state.EntityManager);
        commandBuffer.Dispose();
    }
}