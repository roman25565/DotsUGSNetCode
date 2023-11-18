using Unity.Entities;

public struct ProductionTimeBuffer : IBufferElementData
{
    public int numProduction;
    
    public static implicit operator int(ProductionTimeBuffer e) { return e.numProduction; }
    public static implicit operator ProductionTimeBuffer(int e) { return new ProductionTimeBuffer { numProduction = e }; }
}