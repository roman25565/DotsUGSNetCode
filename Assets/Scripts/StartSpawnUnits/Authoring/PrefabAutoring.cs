using Unity.Entities;
using UnityEngine;



public class PrefabsAutoring : MonoBehaviour
{
    public GameObject prefab;
}

public class PrefabAutoringBaker : Baker<PrefabsAutoring>
{
    public override void Bake(PrefabsAutoring authoring)
    {
            
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new PrefabsComponent()
        {
            prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic)

        });
    }
}

public struct PrefabsComponent : IComponentData
{
    public Entity prefab;
}

