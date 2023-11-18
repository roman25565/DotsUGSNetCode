
using Unity.Entities;


public readonly partial struct UnitDataAspect : IAspect
{
    public readonly RefRW<UnitOwnerComponent> unitOwnerComponent;
    public readonly RefRO<UnitPriorityComponent> oUnitPriorityComponent;
}