using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct AttackSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (stateID, parent, warriorAttackCd, warriorTarget, localTransform, targetPosition) //Attack
                 in SystemAPI.Query<RefRW<StateID>, RefRO<Parent>, RefRW<WarriorAttackCd>,RefRW<WarriorTarget>,RefRO<LocalTransform>,RefRW<TargetPosition>>())
        {
            if (stateID.ValueRO.value != 3)
                continue;

            if (!SystemAPI.Exists(warriorTarget.ValueRO.targetEntity))
            {
                warriorTarget.ValueRW.targetEntity = Entity.Null;
                stateID.ValueRW.value = 0;
                warriorAttackCd.ValueRW.value = 0f;
                continue;
            }
            var networkTime = SystemAPI.GetSingleton<NetworkTime>();
            var currentTick = networkTime.ServerTick; 
            warriorAttackCd.ValueRW.value += Time.deltaTime;
            
            var stoppingDistance = SystemAPI.GetComponent<SquadAttackRange>(parent.ValueRO.Value).attackRange;
            if (SystemAPI.HasComponent<Radius>(warriorTarget.ValueRO.targetEntity))
                stoppingDistance += SystemAPI.GetComponent<Radius>(warriorTarget.ValueRO.targetEntity).value;
            
            var distance = math.distance(SystemAPI.GetComponent<LocalTransform>(warriorTarget.ValueRO.targetEntity).Position, localTransform.ValueRO.Position);
                        
            if (distance > stoppingDistance)
            {
                float3 direction = math.normalize(SystemAPI.GetComponent<Radius>(warriorTarget.ValueRO.targetEntity).value - localTransform.ValueRO.Position);
                
                stateID.ValueRW.value = 4;
                targetPosition.ValueRW.value = SystemAPI.GetComponent<LocalTransform>(warriorTarget.ValueRO.targetEntity).Position - stoppingDistance * direction;
                warriorAttackCd.ValueRW.value = 0f;
            }else if (warriorAttackCd.ValueRO.value >= SystemAPI.GetComponent<SquadAttackCd>(parent.ValueRO.Value).value)
            {
                warriorAttackCd.ValueRW.value = SystemAPI.GetComponent<SquadAttackCd>(parent.ValueRO.Value).value * -2f;
                
                var damage = SystemAPI.GetComponent<SquadDamage>(parent.ValueRO.Value).value;
                var targetHealth = SystemAPI.GetComponent<Health>(warriorTarget.ValueRO.targetEntity).value;
                Debug.Log(damage);
                SystemAPI.SetComponent(warriorTarget.ValueRO.targetEntity, new Health{value = targetHealth - damage});
            }

        }
    }
}