using Unity.Entities;
using UnityEngine;
public class MovementSpeedAuthoring : MonoBehaviour { 
    
    public float value;
}

public class MovementSpeedBaker : Baker<MovementSpeedAuthoring>
{
    public override void Bake(MovementSpeedAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        
        AddComponent(entity, new MovementSpeed { value = authoring.value });
        AddComponent(entity, new ArrivalAction { value = -1 });
        AddComponent(entity, new TargetPosition());
        AddComponent(entity, new StateID{value = 2});
    }
}