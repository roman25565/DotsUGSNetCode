using Unity.Entities;

public readonly partial struct ConstructionAspect : IAspect
{
    private readonly RefRW<ConstructionProgress> _buildingProgress;
    private readonly RefRO<ConstructionTime> _constructionTime;

    public void Building(float deltaTime)
    {
        if (_buildingProgress.ValueRW.progress >= 0 && _buildingProgress.ValueRW.progress < _constructionTime.ValueRO.сonstructionTime) 
            _buildingProgress.ValueRW.progress += deltaTime;
    }
}