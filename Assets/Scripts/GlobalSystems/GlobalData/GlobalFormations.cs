using Unity.Entities;
using UnityEngine;

public class GlobalFormations : MonoBehaviour
{
    public int[] Valuesaa;
}


public class GlobalFormationsBaker : Baker<GlobalFormations>
{
    public override void Bake(GlobalFormations authoringPrefabs)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        DynamicBuffer<GlobalFormationsBuffer> dynamicBuffer = AddBuffer<GlobalFormationsBuffer>(entity);
        dynamicBuffer.ResizeUninitialized(authoringPrefabs.Valuesaa.Length);
        for (int i = 0; i < dynamicBuffer.Length; i++)
        {
            dynamicBuffer.Add(new GlobalFormationsBuffer { Value = authoringPrefabs.Valuesaa[i] });
        }
    }
}

public struct GlobalFormationsBuffer : IBufferElementData
{
    public int Value;
}