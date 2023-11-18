using Unity.Entities;
using Unity.Transforms;

public partial struct WarriorOrderSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (commandId, entity)
                 in SystemAPI.Query<RefRW<WarriorCommandId>>().WithEntityAccess().WithAll<Parent>()) //пришвидшення
        {
            
        }
    }
}