using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct ReadyForInitializeSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ReadyForInitializeCommand>();
    }
    
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (lastPlayerOrder, squadIndState,
                     children, readyForInitialize,formationsId) 
                 in SystemAPI
                     .Query<RefRW<SquadLastPlayerOrder>, RefRW<SquadIndependentState>, DynamicBuffer<Child>, RefRW<ReadyForInitializeCommand>,RefRO<SelectedFormationsId>>()) //move for rotating 
        {
            if (readyForInitialize.ValueRO.value != 1) 
                continue;
            
            var pos0 = SystemAPI.GetComponent<LocalTransform>(children[0].Value).Position;
            float3 pos1 = new float3(0, 0, 0);
            if (squadIndState.ValueRO.targetEntity != Entity.Null)
            {
                pos1 = SystemAPI.GetComponent<LocalTransform>(squadIndState.ValueRO.targetEntity).Position;
            }else if (lastPlayerOrder.ValueRO.targetEntity != Entity.Null) 
            {
                pos1 = SystemAPI.GetComponent<LocalTransform>(lastPlayerOrder.ValueRO.targetEntity).Position;
            }else if (lastPlayerOrder.ValueRO.targetPoz[0] != 0)
            {
                pos1 = lastPlayerOrder.ValueRO.targetPoz;
            }
            var formation = Resources.Load<Formations>($"ScriptableObject/Formations/{formationsId.ValueRO.value}").values;
            var formationPoz = SquadTools.GetPosition(children.Length,pos0,pos1,formation, 0);
            for (int i = 0; i < children.Length; i++)
            {
                SystemAPI.SetComponent(children[i].Value, new StateID{value = 4});//4
                SystemAPI.SetComponent(children[i].Value, new TargetPosition{value = formationPoz[i]});
            }
            readyForInitialize.ValueRW.value = 2;
        }

        foreach (var (lastPlayerOrder, squadIndState,
                     children, readyForInitialize, formationsId, readySoldiersCount,squadAttackRange)
                 in SystemAPI
                     .Query<RefRW<SquadLastPlayerOrder>, RefRW<SquadIndependentState>, DynamicBuffer<Child>,
                         RefRW<ReadyForInitializeCommand>, RefRO<SelectedFormationsId>,RefRW<ReadySoldiersCount>,RefRO<SquadAttackRange>>()) //rotating after move
        {
            if (readyForInitialize.ValueRO.value != 2) 
                continue;
            
            for (int j = 0; j < children.Length; j++)
            {
                if (math.distance(SystemAPI.GetComponent<LocalTransform>(children[j].Value).Position,
                        SystemAPI.GetComponent<TargetPosition>(children[j].Value).value) < .2f && SystemAPI.GetComponent<StateID>(children[j].Value).value == 2)
                {
                    var pos0 = SystemAPI.GetComponent<LocalTransform>(children[0].Value).Position;
                    float3 pos1 = new float3(0, 0, 0);
                    float stoppingDistance = 0;
                    if (squadIndState.ValueRO.targetEntity != Entity.Null)
                    {
                        pos1 = SystemAPI.GetComponent<LocalTransform>(squadIndState.ValueRO.targetEntity).Position;
                        
                    }else if (lastPlayerOrder.ValueRO.targetEntity != Entity.Null)
                    {
                        pos1 = SystemAPI.GetComponent<LocalTransform>(lastPlayerOrder.ValueRO.targetEntity).Position;
                        if (lastPlayerOrder.ValueRO.type == -2)
                        {
                            stoppingDistance += squadAttackRange.ValueRO.attackRange;
                            if (SystemAPI.HasComponent<Radius>(lastPlayerOrder.ValueRO.targetEntity))
                                stoppingDistance += SystemAPI.GetComponent<Radius>(lastPlayerOrder.ValueRO.targetEntity).value;
                        }
                    }else if (lastPlayerOrder.ValueRO.targetPoz[0] != 0)
                    {
                        pos1 = lastPlayerOrder.ValueRO.targetPoz;
                    }
                    var formation = Resources.Load<Formations>($"ScriptableObject/Formations/{formationsId.ValueRO.value}").values;
                    var formationPoz = SquadTools.GetPosition(children.Length,pos1,pos1 - pos0,formation,stoppingDistance);
                    
                    SystemAPI.SetComponent(children[j].Value, new TargetPosition{value = formationPoz[j]});
                    SystemAPI.SetComponent(children[j].Value, new StateID{value = 1});
                    readySoldiersCount.ValueRW.value++;
                }
            }
            if (readySoldiersCount.ValueRO.value == children.Length) // move after rotating
            {
                for (int k = 0; k < children.Length; k++)
                {
                    SystemAPI.SetComponent(children[k].Value, new StateID{value = 4});
                    // SystemAPI.SetComponent(children[i].Value, new TargetPosition{value = formationPoz[i]});
                }
                readySoldiersCount.ValueRW.value = 0;
                readyForInitialize.ValueRW.value = 0;
            }
        }
    }
}