using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct SquadOrderSystem : ISystem
{
    private Entity _targetEntity;
    private int type;
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (lastPlayerOrder, squadIndState, children,readyForInitializeCommand, squadAttackRange) //Squad order to warrior order Initialize
                 in SystemAPI.Query<RefRW<SquadLastPlayerOrder>, RefRW<SquadIndependentState>, DynamicBuffer<Child>,RefRW<ReadyForInitializeCommand>,RefRO<SquadAttackRange>>())
        {
            if(readyForInitializeCommand.ValueRO.value != 0)
                continue;
            if (squadIndState.ValueRO.type == -21)
                continue;
            _targetEntity = Entity.Null;
            type = 0;
            if (lastPlayerOrder.ValueRO.type < 0)
            {
                _targetEntity = lastPlayerOrder.ValueRO.targetEntity;
                type = lastPlayerOrder.ValueRO.type;
                
            }else if (squadIndState.ValueRO.type < 0)
            {
                _targetEntity = squadIndState.ValueRO.targetEntity;
                type = squadIndState.ValueRO.type;
            }

            bool needChange = false;
            switch (type)
            {
                case -1:
                    readyForInitializeCommand.ValueRW.value = 1;
                    
                    needChange = true;
                    break;
                case -2:
                    Debug.Log("tes");
                    if (math.distance(SystemAPI.GetComponent<TargetPosition>(children[0].Value).value, SystemAPI.GetComponent<LocalTransform>(children[0].Value).Position) > 0.1f) 
                        continue;
                    Debug.Log("Yes");
                    foreach (var child in children)
                    {
                        SystemAPI.SetComponent(child.Value, new WarriorTarget{targetEntity = _targetEntity});
                        SystemAPI.SetComponent(child.Value, new StateID{value = 3});
                    }
                    
                    needChange = true;
                    break;
                case -3:
                    break;
                case -4:
                    break;
                case -5:
                    break;
                case -6:
                    break;
            }
            if(!needChange)
                continue;
            if (lastPlayerOrder.ValueRO.type < 0)
            {
                lastPlayerOrder.ValueRW.type *= -1;
                
            }else if (squadIndState.ValueRO.type < 0)
            {
                squadIndState.ValueRW.type *= -1;
            }
        }
    }
}