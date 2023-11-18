using Unity.Entities;
using UnityEngine;

public class WarriorAuthoring : MonoBehaviour
{
}

public class WarriorBaking : Baker<WarriorAuthoring>
{
    public override void Bake(WarriorAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        
        AddComponent(entity, new WarriorHealth());
        AddComponent(entity, new WarriorTarget());
        AddComponent(entity, new WarriorCommandId());
        AddComponent(entity, new WarriorAttackCd());
    }
}