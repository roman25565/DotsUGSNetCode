using Unity.Entities;


public struct ProductionSequenceBuffer : IBufferElementData
{
    public int numProduction;
    
    public static implicit operator int(ProductionSequenceBuffer e) { return e.numProduction; }
    public static implicit operator ProductionSequenceBuffer(int e) { return new ProductionSequenceBuffer { numProduction = e }; }
}