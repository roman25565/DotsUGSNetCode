using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

[GhostComponent(PrefabType=GhostPrefabType.All, SendTypeOptimization=GhostSendType.AllClients)]
public struct Order : IComponentData
{
    // [GhostField]public DynamicBuffer<Entity> buffer;
    [GhostField]public int type;
    [GhostField]public float3 poz1;
    
    [GhostField]public float3 poz2;
    [GhostField]public Entity targetEntity;
}

[GhostComponent(PrefabType = GhostPrefabType.All, SendTypeOptimization = GhostSendType.AllClients)]
public struct OrderEntityBuffer : IBufferElementData
{
    [GhostField]public Entity value;
}