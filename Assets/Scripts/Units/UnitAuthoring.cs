using Unity.Entities;
using UnityEngine;

public class UnitAuthoring : MonoBehaviour
{
    public int priority;
    public int[] skills;
}

public class UnitBaker : Baker<UnitAuthoring>
{
    public override void Bake(UnitAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        
        AddComponent(entity, new UnitPriorityComponent {priority = authoring.priority});
        AddComponent(entity, new UnitOwnerComponent{OwnerId = 1});
        DynamicBuffer<SkillsComponent> dynamicBuffer = AddBuffer<SkillsComponent>(entity);
        dynamicBuffer.ResizeUninitialized(authoring.skills.Length);
        for (int i = 0; i < dynamicBuffer.Length; i++)
        {
            dynamicBuffer[i] = new SkillsComponent{skillsId = authoring.skills[i]};
        }
    }
}