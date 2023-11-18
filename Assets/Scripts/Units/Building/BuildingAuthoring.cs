using Unity.Entities;
using UnityEngine;

public class BuildingAuthoring : MonoBehaviour
{
    [SerializeField]public int[] productionTimes;
    public float progressMax;
}

public class BuildingBaker : Baker<BuildingAuthoring>
{
    public override void Bake(BuildingAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new ConstructionTime() { сonstructionTime = authoring.progressMax });
        AddComponent(entity, new StateID());
        
        DynamicBuffer<ProductionTimeBuffer> dynamicBuffer = AddBuffer<ProductionTimeBuffer>(entity);
        dynamicBuffer.ResizeUninitialized(authoring.productionTimes.Length);
        for (int i = 0; i < dynamicBuffer.Length; i++)
        {
            dynamicBuffer[i] = new ProductionTimeBuffer{numProduction = authoring.productionTimes[i]};
        }
        
        
        AddComponent(entity, new ProductionProgress() { production = 0,productionCoefficient = 1});
        AddComponent(entity, new RallyPointComponent());
        AddBuffer<ProductionSequenceBuffer>(entity);
    }
}