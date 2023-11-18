using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;


public partial class CommandCreatorSystem : SystemBase
{
    private float3 target;
    
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        NativeList<bool> result = new NativeList<bool>(Allocator.TempJob);
        
        new MoveJob
        {
            deltaTime = deltaTime,
            result = result,
        }.Run();
    }
}
public partial struct MoveJob : IJobEntity
{
    public float deltaTime;
    public NativeList<bool> result;
    private void Execute(MoveToPositionAspect moveToPositionAspect)
    {   
        moveToPositionAspect.Move(deltaTime);   
    }
}

public partial struct TestReachedTargetPositionJob : IJobEntity
{
    
    public float3 target;
    
    public void Execute(MoveToPositionAspect moveToPositionAspect)
    {
        moveToPositionAspect.TestReachedTargetPosition(target);
    }
}
