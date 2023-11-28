using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;


[BurstCompile]
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
    
    [BurstCompile]
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
