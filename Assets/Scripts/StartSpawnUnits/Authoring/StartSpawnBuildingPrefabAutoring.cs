using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class StartSpawnBuildingPrefabAutoring : MonoBehaviour
{
    public GameObject prefab;
}

public class StartSpawnBuildingBaker : Baker<StartSpawnBuildingPrefabAutoring>
{
    public override void Bake(StartSpawnBuildingPrefabAutoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new StartSpawnbuildingPrefabComponent()
        {
            prefab = GetEntity(authoring.prefab ,TransformUsageFlags.Dynamic)
        });
    }
}
