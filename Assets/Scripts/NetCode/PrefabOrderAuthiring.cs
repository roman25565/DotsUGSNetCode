using Unity.Entities;
using UnityEngine;

public class PrefabOrderAuthiring : MonoBehaviour
{
    public GameObject prefab;
}

public class PrefabOrderBaker : Baker<PrefabOrderAuthiring>
{
    public override void Bake(PrefabOrderAuthiring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        
        AddComponent(entity, new PrefabOrderComponent
        {
            value =  GetEntity(authoring.prefab ,TransformUsageFlags.Dynamic)
        });
    }
}