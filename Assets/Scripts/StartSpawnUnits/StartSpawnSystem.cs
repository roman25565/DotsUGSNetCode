using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
[BurstCompile]
public partial class StartSpawnUnits : SystemBase
{
    private int ia;
    private StartSpawnbuildingPrefabComponent buildingPrefab;
    private StartSpawnWorkerPrefabComponent workerPrefab;
    private bool spawn_complete = false;
    
    protected override void OnCreate()
    {
        RequireForUpdate<StartSpawnbuildingPrefabComponent>();
        RequireForUpdate<StartSpawnWorkerPrefabComponent>();
        
        SystemAPI.TryGetSingleton<StartSpawnbuildingPrefabComponent>(out this.buildingPrefab);
        SystemAPI.TryGetSingleton<StartSpawnWorkerPrefabComponent>(out this.workerPrefab);
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        if (spawn_complete)
            return;
        
        if (buildingPrefab.prefab == Entity.Null || workerPrefab.prefab == Entity.Null)
        {
            SystemAPI.TryGetSingleton<StartSpawnbuildingPrefabComponent>(out this.buildingPrefab);
            SystemAPI.TryGetSingleton<StartSpawnWorkerPrefabComponent>(out this.workerPrefab);
            if (buildingPrefab.prefab == Entity.Null || workerPrefab.prefab == Entity.Null)
            {
                Debug.Log("prefabs 404");
                return;
            }
        }
        if (ia > 4)
        {
            return;
        }
        
        foreach (var (normalMesh,entity) in SystemAPI.Query<RefRW<NormalMesh>>().WithEntityAccess())
        {
            normalMesh.ValueRW.normalMesh =
                SystemAPI.GetComponent<MaterialMeshInfo>(SystemAPI.GetBuffer<Child>(entity)[0].Value)
                    .Mesh;
            ia++;
        };//need
        
        
        var i = 0;
        foreach (var spawnointTag in SystemAPI.Query<SpawnpointTag>())
        {
            var result = GameDataManager.Instance.SpawnPointHasPlayer(i);
            Debug.Log("result: " + result);
            bool spawnPointNotHasPlayer = result == null;
            i++;
            if (spawnPointNotHasPlayer)
                continue;
            var myId = result ?? 0;

            Debug.Log("myId: " + myId);
            var SpawnPoint = spawnointTag.localTransform;
            var projectileEntity = EntityManager.Instantiate(buildingPrefab.prefab);

            SystemAPI.SetComponent(projectileEntity, new UnitOwnerComponent { OwnerId = myId });
            SystemAPI.SetComponent(projectileEntity, new LocalTransform()
            {
                Position = new float3(SpawnPoint),
                Rotation = new float4(0, 0, 0, 1),
                Scale = 1
            });
            var localTransform = SystemAPI.GetComponent<LocalTransform>(projectileEntity);
            float3 forwardDirection =
                math.mul(localTransform.Rotation.value, new float3(0, 0, 5)); // Орієнтація сутності
            float3 pointInFront = localTransform.Position + forwardDirection;
            SystemAPI.SetComponent(projectileEntity, new RallyPointComponent { position = pointInFront });
            var newWorker = EntityManager.Instantiate(workerPrefab.prefab);
            SystemAPI.SetComponent(newWorker, new UnitOwnerComponent { OwnerId = myId });
            SystemAPI.SetComponent(newWorker, new LocalTransform()
            {
                Position = new float3(
                    x: SpawnPoint.x - 2.1f,
                    y: SpawnPoint.y,
                    z: SpawnPoint.z + 3f),
                Rotation = new float4(0, 0, 0, 1),
                Scale = 1
            });
            SystemAPI.SetComponent(newWorker, new TargetPosition
            {
                value = new float3(
                    x: SpawnPoint.x - 2f,
                    y: SpawnPoint.y,
                    z: SpawnPoint.z + 3f)
            });
            var newWorker2 = EntityManager.Instantiate(workerPrefab.prefab);
            SystemAPI.SetComponent(newWorker2, new UnitOwnerComponent { OwnerId = myId });
            SystemAPI.SetComponent(newWorker2, new LocalTransform()
            {
                Position = new float3(
                    x: SpawnPoint.x + 4,
                    y: SpawnPoint.y,
                    z: SpawnPoint.z + 3),
                Rotation = new float4(0, 0, 0, 1),
                Scale = 1
            });
            SystemAPI.SetComponent(newWorker2, new TargetPosition
            {
                value = new float3(
                    x: SpawnPoint.x + 4.1f,
                    y: SpawnPoint.y,
                    z: SpawnPoint.z + 3f)
            });
        }

        spawn_complete = true;
        
    }
}
