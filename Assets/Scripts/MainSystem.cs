using Unity.Burst;
using Unity.Entities;

[DisableAutoCreation] 
[BurstCompile]
public partial class MainSystem : SystemBase
{
    [BurstCompile]
    protected override void OnCreate()
    {
        
    }
    
    
    [BurstCompile]
    protected override void OnUpdate()
    {
        // World.CreateSystem<StartSpawnUnits>();
    }
}