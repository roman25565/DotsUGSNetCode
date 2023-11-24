using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct SquadSystem : ISystem
{

    public void OnUpdate(ref SystemState state)
    {
        foreach (var (movementSpeed, entity)
                 in SystemAPI.Query<RefRW<MovementSpeed>>().WithEntityAccess().WithAll<Parent>()) //пришвидшення
        {
            var mainSquad = SystemAPI.GetComponent<Parent>(entity).Value;
            if (!SystemAPI.HasComponent<SquadSpeedMaxAcceleration>(mainSquad))
                continue;

            var speedMaxC = SystemAPI.GetComponent<SquadSpeedMaxAcceleration>(mainSquad);

            if (movementSpeed.ValueRO.value < speedMaxC.maxSpeed)
            {
                movementSpeed.ValueRW.value += Time.deltaTime * speedMaxC.acceleration;
            }

            //need
        }

        foreach (var (lastPlayerOrder, squadIndState, children, startPosition, maxHealth) //Setup
                 in SystemAPI
                     .Query<RefRO<SquadLastPlayerOrder>, RefRW<SquadIndependentState>, DynamicBuffer<Child>,
                         RefRO<SquadStartPosition>,RefRO<SquadMaxHealth>>())
        {
            //NEED normal squad tools
            if (squadIndState.ValueRO.type != -21)
                continue;
            squadIndState.ValueRW.type = 1;
            foreach (var child in children)
            {
                SystemAPI.SetComponent(child.Value, new Health { value = maxHealth.ValueRO.maxHealth });
                SystemAPI.SetComponent(child.Value, new StateID { value = 1 });
                SystemAPI.SetComponent(child.Value, new TargetPosition { value = lastPlayerOrder.ValueRO.targetPoz });
                SystemAPI.SetComponent(child.Value, new LocalTransform
                {
                    Position = startPosition.ValueRO.value, Scale = 1,
                    Rotation = new quaternion(0, 0, 0, 1)
                });
            }
        }
    }
}
