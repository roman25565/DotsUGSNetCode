using Unity.Entities;

public struct ProductionProgress : IComponentData
{
    public float production;
    public float productionCoefficient;
}