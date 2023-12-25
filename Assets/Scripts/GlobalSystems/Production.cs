using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct Production : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        var productionJob = new ProductionJob
        {
            deltaTime = deltaTime,
        };
        state.Dependency = productionJob.ScheduleParallel(state.Dependency);
    }
    
    [BurstCompile]
    partial struct ProductionJob : IJobEntity
    {
        // public NetworkTick tick;
        public float deltaTime;
        
        public void Execute(DynamicBuffer<ProductionSequenceBuffer> productionSequenceBuffer, ref ProductionProgress productionProgress)
        {
            if (productionSequenceBuffer.Length > 0)
            {
                productionProgress.production += deltaTime *
                                                 productionProgress.productionCoefficient;
            }
        }
    }
}