using Unity.Entities;
// [BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct InputToOutputEntityPoz : ISystem
{
    // [BurstCompile]
    // public void OnCreate(ref SystemState state)
    // {
    //     state.RequireForUpdate<InputOrder>();
    //     state.RequireForUpdate<InputOrderEntityPoz>();
    // }
    
    // [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (inputOrderEntityPozes, orderEntityPoz) in SystemAPI
                     .Query<RefRO<InputOrderEntityPoz>, RefRW<OutputOrderEntityPoz>>())
        {
            orderEntityPoz.ValueRW.center = inputOrderEntityPozes.ValueRO.center;
            orderEntityPoz.ValueRW.size = inputOrderEntityPozes.ValueRO.size;
        }
    }
}