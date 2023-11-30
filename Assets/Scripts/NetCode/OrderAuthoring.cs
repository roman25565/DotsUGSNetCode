using Unity.Entities;
using UnityEngine;

public class OrderAuthoring : MonoBehaviour
{
        
}

public class OrderBaker : Baker<OrderAuthoring>
{
    public override void Bake(OrderAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        
        AddComponent(entity, new Order());
        AddBuffer<OrderEntityBuffer>(entity);
    }
}