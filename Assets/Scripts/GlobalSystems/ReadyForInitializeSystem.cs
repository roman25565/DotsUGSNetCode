using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct ReadyForInitializeSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (lastPlayerOrder, squadIndState,
                     children, readyForInitialize,formationsId) 
                 in SystemAPI
                     .Query<RefRW<SquadLastPlayerOrder>, RefRW<SquadIndependentState>, DynamicBuffer<Child>, RefRW<ReadyForInitializeCommand>,RefRO<SelectedFormationsId>>())
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
            var formationPoz = SquadTools.getPosition(children.Length,pos0,pos1,formation);
            for (int i = 0; i < children.Length; i++)
            {
                SystemAPI.SetComponent(children[i].Value, new StateID{value = 4});
                SystemAPI.SetComponent(children[i].Value, new TargetPosition{value = formationPoz[i]});
            }
            readyForInitialize.ValueRW.value = 2;
        }

        // foreach (var (lastPlayerOrder, squadIndState,
        //              children, readyForInitialize, formationsId)
        //          in SystemAPI
        //              .Query<RefRW<SquadLastPlayerOrder>, RefRW<SquadIndependentState>, DynamicBuffer<Child>,
        //                  RefRW<ReadyForInitializeCommand>, RefRO<SelectedFormationsId>>())
        // {
        //     if (readyForInitialize.ValueRO.value != 2) 
        //         continue;
        //     int i = 0;
        //     foreach (var child in children)
        //     {
        //         if (math.distance(SystemAPI.GetComponent<LocalTransform>(child.Value).Position,
        //                 SystemAPI.GetComponent<TargetPosition>(child.Value).value) < .5f)
        //         {
        //             var pos0 = SystemAPI.GetComponent<LocalTransform>(children[0].Value).Position;
        //             float3 pos1 = new float3(0, 0, 0);
        //             if (squadIndState.ValueRO.targetEntity != Entity.Null)
        //             {
        //                 pos1 = SystemAPI.GetComponent<LocalTransform>(squadIndState.ValueRO.targetEntity).Position;
        //             }else if (lastPlayerOrder.ValueRO.targetEntity != Entity.Null) 
        //             {
        //                 pos1 = SystemAPI.GetComponent<LocalTransform>(lastPlayerOrder.ValueRO.targetEntity).Position;
        //             }else if (lastPlayerOrder.ValueRO.targetPoz[0] != 0)
        //             {
        //                 pos1 = lastPlayerOrder.ValueRO.targetPoz;
        //             }
        //             var formation = Resources.Load<Formations>($"ScriptableObject/Skills/{formationsId.ValueRO.value}").values;
        //             var formationPoz = SquadTools.getPosition(children.Length,pos1,new float3(0,0,0),formation);
        //             SystemAPI.SetComponent(children[i].Value, new StateID{value = 1});
        //             SystemAPI.SetComponent(children[i].Value, new TargetPosition{value = formationPoz[i]});
        //             i++;
        //         }
        //     }
        //         
        //     if (i == children.Length)
        //         readyForInitialize.ValueRW.value = 3;
        //     
        // }
        // foreach (var (lastPlayerOrder, squadIndState,
        //              children, readyForInitialize, formationsId)
        //          in SystemAPI
        //              .Query<RefRW<SquadLastPlayerOrder>, RefRW<SquadIndependentState>, DynamicBuffer<Child>,
        //                  RefRW<ReadyForInitializeCommand>, RefRO<SelectedFormationsId>>())
        // {
        //     if (readyForInitialize.ValueRO.value != 3) 
        //         continue;
        //     
        //     var pos0 = SystemAPI.GetComponent<LocalTransform>(children[0].Value).Position;
        //     float3 pos1 = new float3(0, 0, 0);
        //     if (squadIndState.ValueRO.targetEntity != Entity.Null)
        //     {
        //         pos1 = SystemAPI.GetComponent<LocalTransform>(squadIndState.ValueRO.targetEntity).Position;
        //     }else if (lastPlayerOrder.ValueRO.targetEntity != Entity.Null) 
        //     {
        //         pos1 = SystemAPI.GetComponent<LocalTransform>(lastPlayerOrder.ValueRO.targetEntity).Position;
        //     }else if (lastPlayerOrder.ValueRO.targetPoz[0] != 0)
        //     {
        //         pos1 = lastPlayerOrder.ValueRO.targetPoz;
        //     }
        //     var formation = Resources.Load<Formations>($"ScriptableObject/Skills/{formationsId.ValueRO.value}").values;
        //     var formationPoz = SquadTools.getPosition(children.Length,pos0,pos1,formation);
        //     for (int i = 0; i < children.Length; i++)
        //     {
        //         SystemAPI.SetComponent(children[i].Value, new StateID{value = 1});
        //         SystemAPI.SetComponent(children[i].Value, new TargetPosition{value = formationPoz[i]});
        //     }
        //     
        //     
        //     readyForInitialize.ValueRW.value = 0;
        // }
    }
}