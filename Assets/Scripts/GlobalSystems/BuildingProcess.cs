using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public partial struct BuildingProcess : ISystem
{
    
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (localTransform,targetPosition,arrivalAction,entity) 
                 in SystemAPI.Query<RefRO<LocalTransform>,RefRO<TargetPosition>,RefRW<ArrivalAction>>().WithEntityAccess())
        {
            if (math.distance(localTransform.ValueRO.Position, targetPosition.ValueRO.value) < .5f)
            {
                if(arrivalAction.ValueRW.value != -2)
                    continue;
                var workerMesh = SystemAPI.GetBuffer<Child>(entity)[0].Value;
                SystemAPI.SetComponent(workerMesh,
                    new MaterialMeshInfo
                        { Mesh = 0, Material = SystemAPI.GetComponent<MaterialMeshInfo>(workerMesh).Material });
                arrivalAction.ValueRW.value = -3;

                float b = 1f;
                var mainBuildEntity = Entity.Null;
                foreach (var (buildLocalTransform, constructionProgress, buildEntity)
                         in SystemAPI.Query<RefRO<LocalTransform>, RefRW<ConstructionProgress>>()
                             .WithEntityAccess())
                {
                    float a = math.distance(buildLocalTransform.ValueRO.Position, localTransform.ValueRO.Position);

                    if (a < b)
                    {
                        b = a;
                        mainBuildEntity = buildEntity;
                    }
                }

                SystemAPI.SetComponent(mainBuildEntity, new ConstructionProgress {
                        progress = 0,
                        myBilder = SystemAPI.GetComponent<ConstructionProgress>(mainBuildEntity).myBilder
                    });
            }
        }
        
        
        float deltaTime = SystemAPI.Time.DeltaTime;
        
        new BuildingJob
        {
            deltaTime = deltaTime,
        }.Run();
    }
}

public partial struct BuildingJob : IJobEntity
{
    public float deltaTime;
    private void Execute(ConstructionAspect constructionAspect)
    {
        constructionAspect.Building(deltaTime);
    }
}