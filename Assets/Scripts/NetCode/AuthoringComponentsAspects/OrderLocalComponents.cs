using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(PrefabType=GhostPrefabType.Client)]
public struct OrderLocal : IComponentData
{
    public int type;
    public float3 poz1;
    public float3 poz2;
    public Entity targetEntity;
}

[GhostComponent(PrefabType=GhostPrefabType.Client)]
public struct OutputOrderEntityPozLocal : IComponentData
{
    public Vector3 center;
    public Vector3 size;
}