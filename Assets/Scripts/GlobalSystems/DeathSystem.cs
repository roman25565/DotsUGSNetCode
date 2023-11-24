using Unity.Collections;
using Unity.Entities;

public partial struct DeathSystem : ISystem
{
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