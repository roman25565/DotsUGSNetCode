using Unity.Entities;

public struct ConstructionProgress : IComponentData
{
    public float progress;
    public Entity myBilder;
}