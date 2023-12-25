using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct DeathSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Health>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach (var (health,entity)
                 in SystemAPI.Query<RefRO<Health>>().WithEntityAccess())
        {
            if (health.ValueRO.value <= 0)
            {
                ecb.DestroyEntity(entity);
            }
        }
        state.Dependency.Complete();

        // To ensure deterministic playback order,
        // the commands are first sorted by their sort keys.
        ecb.Playback(state.EntityManager);

        ecb.Dispose();
    }
}