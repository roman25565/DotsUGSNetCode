using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

[GhostComponent(PrefabType=GhostPrefabType.AllPredicted)]
public struct InputOrder : IInputComponentData
{
    public int type;
    public float3 poz1;
    
    public float3 poz2;
    public Entity targetEntity;
}

public struct OutputOrder : IComponentData
{
    [GhostField]public int type;
    [GhostField]public float3 poz1;
    
    [GhostField]public float3 poz2;
    [GhostField]public Entity targetEntity;
}

[GhostComponent(PrefabType=GhostPrefabType.AllPredicted)]
public struct InputOrderEntityPoz : IInputComponentData
{
    public Vector3 center;
    public Vector3 size;
}

public struct OutputOrderEntityPoz : IComponentData
{
    [GhostField] public Vector3 center;
    [GhostField] public Vector3 size;
}

[GhostComponent(PrefabType=GhostPrefabType.Client)]
public struct LocalOrderEntityBuffer : IBufferElementData
{
    public Entity value;
}

