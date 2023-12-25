
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[BurstCompile]
public partial class StartSpawnUnits : SystemBase
{
    private int ia;
    protected override void OnCreate()
    {
        RequireForUpdate<StartSpawnbuildingPrefabComponent>();
        RequireForUpdate<StartSpawnWorkerPrefabComponent>();
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        if (ia > 4)
        {
            return;
        }
        StartSpawnbuildingPrefabComponent buildingPrefab = SystemAPI.GetSingleton<StartSpawnbuildingPrefabComponent>();
        StartSpawnWorkerPrefabComponent workerPrefab = SystemAPI.GetSingleton<StartSpawnWorkerPrefabComponent>();
        

        int buildingCount = EntityManager.CreateEntityQuery(typeof(UnitOwnerComponent)).CalculateEntityCount();

        foreach (var (normalMesh,entity) in SystemAPI.Query<RefRW<NormalMesh>>().WithEntityAccess())
        {
            normalMesh.ValueRW.normalMesh =
                SystemAPI.GetComponent<MaterialMeshInfo>(SystemAPI.GetBuffer<Child>(entity)[0].Value)
                    .Mesh;
            ia++;
        };
        
        if (buildingCount < EntityManager.CreateEntityQuery(typeof(SpawnpointTag)).CalculateEntityCount())
        {
            foreach (var spawnointTag in SystemAPI.Query<SpawnpointTag>())
            {
                var SpawnPoint = spawnointTag.localTransform;
                var projectileEntity = EntityManager.Instantiate(buildingPrefab.prefab);
                
                SystemAPI.SetComponent(projectileEntity, new UnitOwnerComponent{OwnerId = 1});
                SystemAPI.SetComponent(projectileEntity, new LocalTransform()
                {
                    Position = new float3(SpawnPoint),
                    Rotation = new float4(0, 0, 0, 1),
                    Scale = 1
                });
                var localTransform = SystemAPI.GetComponent<LocalTransform>(projectileEntity);
                float3 forwardDirection = math.mul(localTransform.Rotation.value, new float3(0, 0, 5)); // Орієнтація сутності
                float3 pointInFront = localTransform.Position + forwardDirection;
                SystemAPI.SetComponent(projectileEntity, new RallyPointComponent{position = pointInFront});
                var newWorker = EntityManager.Instantiate(workerPrefab.prefab);
                SystemAPI.SetComponent(newWorker, new UnitOwnerComponent{OwnerId = 1});
                // await SpawnChildAsync(newWorker, new float3(x:SpawnPoint.x - 2.1f, y:SpawnPoint.y, z:SpawnPoint.z + 3f));
                SystemAPI.SetComponent(newWorker, new LocalTransform()
                    {
                        Position = new float3(
                            x:SpawnPoint.x - 2.1f, 
                            y:SpawnPoint.y,
                            z:SpawnPoint.z + 3f),
                        Rotation = new float4(0,0,0,1),
                        Scale = 1
                    }
                );
                SystemAPI.SetComponent(newWorker, new TargetPosition{value = new float3(
                    x:SpawnPoint.x - 2f, 
                    y:SpawnPoint.y,
                    z:SpawnPoint.z + 3f)});
                var newWorker2 = EntityManager.Instantiate(workerPrefab.prefab);
                SystemAPI.SetComponent(newWorker2, new UnitOwnerComponent{OwnerId = 1});
                // await SpawnChildAsync(newWorker2, new float3(x:SpawnPoint.x + 4, y:SpawnPoint.y, z:SpawnPoint.z + 3));
                SystemAPI.SetComponent(newWorker2, new LocalTransform()
                    {
                        Position = new float3(
                            x:SpawnPoint.x + 4, 
                            y:SpawnPoint.y,
                            z:SpawnPoint.z + 3),
                        Rotation = new float4(0,0,0,1),
                        Scale = 1
                    }
                );
                SystemAPI.SetComponent(newWorker2, new TargetPosition{value = new float3(
                    x:SpawnPoint.x + 4.1f, 
                    y:SpawnPoint.y,
                    z:SpawnPoint.z + 3f)});
            }
        }
        
    }
}
