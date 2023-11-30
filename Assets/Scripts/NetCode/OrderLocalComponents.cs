using Unity.Entities;
using Unity.Mathematics;

public struct OrderLocal : IComponentData
{
    public int id;
    public int type;
    public float3 poz1;
    public float3 poz2;
    public Entity targetEntity;
}

public struct OrderEntityBufferLocal : IBufferElementData
{
    public Entity value;
}

public struct ID : IComponentData
{
    public int value;
}