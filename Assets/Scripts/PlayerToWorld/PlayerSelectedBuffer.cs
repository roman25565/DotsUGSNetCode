
using Unity.Entities;
using UnityEngine.UI;

public struct PlayerSelectedBuffer : IBufferElementData
{
    public Entity selectedEntity;

    // public void Select(Entity entity)
    // {
    //     selectedEntity.
    // }
    
    public static implicit operator Entity(PlayerSelectedBuffer e) { return e.selectedEntity; }
    public static implicit operator PlayerSelectedBuffer(Entity e) { return new PlayerSelectedBuffer { selectedEntity = e }; }
}