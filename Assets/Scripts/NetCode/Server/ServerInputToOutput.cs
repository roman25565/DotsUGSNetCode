using Unity.Burst;
using Unity.Entities;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct InputToOutputOrder : ISystem
{
    // [BurstCompile]
    // public void OnCreate(ref SystemState state)
    // {
    //     state.RequireForUpdate<InputOrder>();
    //     state.RequireForUpdate<InputOrderEntityPoz>();
    //     state.RequireForUpdate<ServerInputToOutputEntityPoz>();
    // }
    
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (inputOrder,inputOrderEntityBuffers,outputOrder) in SystemAPI
                     .Query<RefRO<InputOrder>, RefRO<InputOrderEntityPoz>,RefRW<OutputOrder>>())
        {
            outputOrder.ValueRW.type = inputOrder.ValueRO.type;
            outputOrder.ValueRW.poz1 = inputOrder.ValueRO.poz1;
            outputOrder.ValueRW.poz2 = inputOrder.ValueRO.poz2;
            outputOrder.ValueRW.targetEntity = inputOrder.ValueRO.targetEntity;
        }
    }
}



