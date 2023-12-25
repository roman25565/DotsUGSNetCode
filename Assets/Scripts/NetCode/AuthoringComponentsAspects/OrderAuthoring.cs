using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class OrderAuthoring : MonoBehaviour
{
        
}

public class OrderBaker : Baker<OrderAuthoring>
{
    public override void Bake(OrderAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        
        AddComponent<InputOrder>(entity);
        AddComponent<OutputOrder>(entity);
        
        AddComponent<OrderLocal>(entity);
        
        
        AddComponent<InputOrderEntityPoz>(entity);
        AddComponent<OutputOrderEntityPoz>(entity);
        
        AddComponent<OutputOrderEntityPozLocal>(entity);
        
        
        AddBuffer<LocalOrderEntityBuffer>(entity);
    }
}