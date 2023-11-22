using Unity.Entities;
using Unity.Mathematics;

public struct ConstructionProgress : IComponentData
{
    public float progress;
    public Entity myBilder;
}
public struct ConstructionTime : IComponentData
{
    public float value;
}
public struct ProductionProgress : IComponentData
{
    public float production;
    public float productionCoefficient;
}
public struct ProductionSequenceBuffer : IBufferElementData
{
    public int numProduction;
    
    public static implicit operator int(ProductionSequenceBuffer e) { return e.numProduction; }
    public static implicit operator ProductionSequenceBuffer(int e) { return new ProductionSequenceBuffer { numProduction = e }; }
}
public struct ProductionTimeBuffer : IBufferElementData
{
    public int numProduction;
    
    public static implicit operator int(ProductionTimeBuffer e) { return e.numProduction; }
    public static implicit operator ProductionTimeBuffer(int e) { return new ProductionTimeBuffer { numProduction = e }; }
}
public struct RallyPointComponent : IComponentData
{
    public float3 position;
}
public struct Radius : IComponentData
{
    public float value;
}