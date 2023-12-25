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
        PrefabOrderComponent component = default(PrefabOrderComponent);
        component.value = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic);
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, component);
    }
}