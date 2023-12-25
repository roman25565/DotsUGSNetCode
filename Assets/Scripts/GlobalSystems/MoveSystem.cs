using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct MoveSystem : ISystem
{
    private float3 target;
    
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        // NativeList<bool> result = new NativeList<bool>(Allocator.TempJob);

        var moveJob = new MoveJob
        {
            deltaTime = deltaTime,
            // result = result,
        };
        state.Dependency = moveJob.ScheduleParallel(state.Dependency);
    }
    
    public partial struct MoveJob : IJobEntity
    {
        public float deltaTime;
        // public NativeList<bool> result;
        private void Execute(MoveToPositionAspect moveToPositionAspect)
        {   
            moveToPositionAspect.Move(deltaTime);   
        }
    }
}
