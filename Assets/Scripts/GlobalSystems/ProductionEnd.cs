using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct ProductionEnd : ISystem
{
    private int _myId;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ProductionProgress>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _myId = SystemAPI.GetSingleton<PlayerData>().playerid;
        DynamicBuffer<BuilderPrefabs> _builderPrefabsComponents = SystemAPI.GetSingletonBuffer<BuilderPrefabs>();
        foreach (var ( progress, timeBuffer, sequenceBuffer, skills, localTransform, rallyPointComponent)
                 in SystemAPI.Query<RefRW<ProductionProgress>,DynamicBuffer<ProductionTimeBuffer>,DynamicBuffer<ProductionSequenceBuffer>,DynamicBuffer<SkillsComponent>,RefRO<LocalTransform>,
                     RefRO<RallyPointComponent>>())
        {
            
            if(sequenceBuffer.Length == 0)
                continue;
            if (progress.ValueRO.production == 0)
                continue;
            if (progress.ValueRO.production < timeBuffer[sequenceBuffer[0]])
                continue;
            

            var builderPrefabs = _builderPrefabsComponents[skills[sequenceBuffer[0]]];

            var NewUnit = state.EntityManager.Instantiate(builderPrefabs.Value);
            SystemAPI.SetComponent(NewUnit, new UnitOwnerComponent { OwnerId = _myId });
            
            bool isSquad = SystemAPI.HasComponent<SquadMaxHealth>(NewUnit);
            if (isSquad)
            {
                SystemAPI.SetComponent(NewUnit, new SquadLastPlayerOrder { targetPoz = rallyPointComponent.ValueRO.position });
                SystemAPI.SetComponent(NewUnit, new SquadStartPosition { value = localTransform.ValueRO.Position});
            }
            else
            {
                Debug.Log("WTF");
                SystemAPI.SetComponent(NewUnit, new LocalTransform
                {
                    Position = new float3(localTransform.ValueRO.Position),
                    Scale = 1f
                });
                SystemAPI.SetComponent(NewUnit, new TargetPosition { value = rallyPointComponent.ValueRO.position });
            }

            progress.ValueRW.production = 0;
            sequenceBuffer.RemoveAt(0);
        }
    }
    
}