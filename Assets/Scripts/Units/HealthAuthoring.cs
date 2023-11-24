using Unity.Entities;
using UnityEngine;

public class HealthAuthoring : MonoBehaviour
{
    public float value;
}


public class HealthBaker : Baker<HealthAuthoring>
{
    public override void Bake(HealthAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        
        AddComponent(entity, new Health{value = authoring.value});
    }
}

