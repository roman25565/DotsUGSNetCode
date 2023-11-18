using Unity.Entities;
using Unity.Transforms;

public readonly partial struct ProductionAspect : IAspect
{
    public readonly RefRW<ProductionProgress> _productionProgress;
    public readonly DynamicBuffer<ProductionTimeBuffer> _productionTimeBuffer;
    public readonly DynamicBuffer<ProductionSequenceBuffer> _productionSequenceBuffer;
    public readonly DynamicBuffer<SkillsComponent> _skills;
    public readonly RefRO<LocalTransform> localTransform;
    public readonly RefRO<RallyPointComponent> rallyPointComponent;
}