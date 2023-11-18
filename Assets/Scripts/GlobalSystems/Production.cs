using Unity.Entities;
using UnityEngine;

public partial struct Production : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (productionSequenceBuffer, productionProgress)
                 in SystemAPI.Query<DynamicBuffer<ProductionSequenceBuffer>,RefRW<ProductionProgress>>())
        {
            if (productionSequenceBuffer.Length > 0)
            {
                productionProgress.ValueRW.production += Time.deltaTime *
                    productionProgress.ValueRO.productionCoefficient;
            }
        }
    }
}