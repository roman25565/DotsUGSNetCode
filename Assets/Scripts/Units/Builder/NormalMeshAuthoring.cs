using Unity.Entities;
using UnityEngine;

public class NormalMeshAuthoring : MonoBehaviour
{
}

public class NormalMeshbaker : Baker<NormalMeshAuthoring>
{
    public override void Bake(NormalMeshAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new NormalMesh());
    }
}