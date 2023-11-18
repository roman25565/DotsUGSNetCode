using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnpointTagAuthoring : MonoBehaviour
{
    public Transform transform;
}

public class SpawnpointsBaker : Baker<SpawnpointTagAuthoring>
{
    public override void Bake(SpawnpointTagAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new SpawnpointTag()
        {
            localTransform = authoring.transform.position});
        }
}
