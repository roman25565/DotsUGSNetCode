using Unity.Entities;
using Unity.Mathematics;

public struct MovementSpeed : IComponentData
{
    public float value;
}

public struct TargetPosition : IComponentData
{ 
    public float3 value;
}

public struct ArrivalAction : IComponentData
{
    public int value;
}

public struct StateID : IComponentData
{
    public int value;
}