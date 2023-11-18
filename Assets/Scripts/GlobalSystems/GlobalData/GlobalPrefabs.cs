using Unity.Entities;
using UnityEngine;

public class GlobalPrefabs : MonoBehaviour
{
    public GameObject[] Values;
}

public class BuilderBaker : Baker<GlobalPrefabs>
{
    public override void Bake(GlobalPrefabs authoringPrefabs)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        DynamicBuffer<BuilderPrefabsComponent> dynamicBuffer = AddBuffer<BuilderPrefabsComponent>(entity);
        dynamicBuffer.ResizeUninitialized(authoringPrefabs.Values.Length);
        for (int i = 0; i < dynamicBuffer.Length; i++)
        {
            dynamicBuffer[i] = new BuilderPrefabsComponent{Value = GetEntity(authoringPrefabs.Values[i])};
        }
    }
}

public struct BuilderPrefabsComponent : IBufferElementData
{
    public Entity Value;
}