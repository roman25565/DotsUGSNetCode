using Unity.Entities;

public struct WarriorTarget : IComponentData
{
    public Entity targetEntity;
}
public struct WarriorCommandId : IComponentData
{
    public int value;
}
public struct WarriorAttackCd : IComponentData
{
    public float value;
}