using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct SpawnpointTag : IComponentData
{
    public float3 localTransform;
}
