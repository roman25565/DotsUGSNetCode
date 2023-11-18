using Unity.Entities;

public struct UnitPriorityComponent : IComponentData
{
    public int priority;
}

public struct UnitOwnerComponent : IComponentData
{
    public int OwnerId;
}

public struct SkillsComponent : IBufferElementData
{
    public int skillsId;
    
    public static implicit operator int(SkillsComponent e) { return e.skillsId; }
    public static implicit operator SkillsComponent(int e) { return new SkillsComponent { skillsId = e }; }
}