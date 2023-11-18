using Unity.Entities;
using UnityEngine;

public class SelectedTagAuthoring : MonoBehaviour { 
}

public class SelectedTagBaker : Baker<SelectedTagAuthoring>
{
    public override void Bake(SelectedTagAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        
        AddComponent<SelectedTag>(entity);
    }
}

public class SelectedTag : IComponentData
{
}