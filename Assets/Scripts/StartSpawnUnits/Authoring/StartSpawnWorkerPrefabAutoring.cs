using Unity.Entities;
using UnityEngine;

public class StartSpawnWorkerPrefabAutoring : MonoBehaviour
{
    public GameObject prefab;
}

public class StartSpawnWorkerBaker : Baker<StartSpawnWorkerPrefabAutoring>
{
    public override void Bake(StartSpawnWorkerPrefabAutoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new StartSpawnWorkerPrefabComponent()
        {
            prefab = GetEntity(authoring.prefab ,TransformUsageFlags.Dynamic)
        });
    }
}