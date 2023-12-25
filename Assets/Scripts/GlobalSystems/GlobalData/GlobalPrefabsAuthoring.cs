using Unity.Entities;
using UnityEngine;

public class GlobalPrefabsAuthoring : MonoBehaviour
{
    public GameObject[] values;
}
public class BuilderPrefabsBaker : Baker<GlobalPrefabsAuthoring>
{
    public override void Bake(GlobalPrefabsAuthoring authoringPrefabs)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        DynamicBuffer<BuilderPrefabs> dynamicBuffer = AddBuffer<BuilderPrefabs>(entity);
        dynamicBuffer.ResizeUninitialized(authoringPrefabs.values.Length);
        for (int i = 0; i < dynamicBuffer.Length; i++)
        {
            dynamicBuffer[i] = new BuilderPrefabs{Value = GetEntity(authoringPrefabs.values[i])};
        }
    }
}