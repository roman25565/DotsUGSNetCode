using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
public partial class ProductionEnd : SystemBase
{
    

    protected override async void OnUpdate()
    {
        DynamicBuffer<BuilderPrefabsComponent> _builderPrefabsComponents = SystemAPI.GetSingletonBuffer<BuilderPrefabsComponent>();
        int _myId = SystemAPI.GetSingleton<PlayerData>().playerid;
        foreach ((RefRW<ProductionProgress> progress, DynamicBuffer<ProductionTimeBuffer> timeBuffer, 
                     DynamicBuffer<ProductionSequenceBuffer> sequenceBuffer, DynamicBuffer<SkillsComponent> skills, 
                     RefRO<LocalTransform> localTransform,
                     RefRO<RallyPointComponent> rallyPointComponent)
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

            var NewUnit = EntityManager.Instantiate(builderPrefabs.Value);
            SystemAPI.SetComponent(NewUnit, new UnitOwnerComponent { OwnerId = _myId });
            
            bool isSquad = SystemAPI.HasComponent<SquadMaxHealth>(NewUnit);
            if (isSquad)
            {
                // var ecb = new EntityCommandBuffer(Allocator.Temp);
                // var shouldLoadNext = false;
                // Entities.ForEach((Entity entity, in DynamicBuffer<Child> childBuffer,in SquadMaxHealth maxHealth) =>
                // {
                //     // foreach (var child in childBuffer)
                //     // {
                //     //     ecb.SetComponent(child.Value, new LocalTransform
                //     //     {
                //     //         Position = new float3(position),
                //     //         Scale = 1f
                //     //     });
                //     //     ecb.SetComponent(child.Value, new WarriorIsRotating { value = true });
                //     //     ecb.SetComponent(child.Value, new TargetPosition { value = targetPosition });
                //     // }
                // }).Run();
                // ecb.Playback(EntityManager);
                // await SpawnChildAsync(NewUnit,localTransform.ValueRO.Position,ecb);
                SystemAPI.SetComponent(NewUnit, new SquadLastPlayerOrder { targetPoz = rallyPointComponent.ValueRO.position });
                SystemAPI.SetComponent(NewUnit, new SquadStartPosition { value = localTransform.ValueRO.Position});
            }
            else
            {
                SystemAPI.SetComponent(NewUnit, new LocalTransform
                {
                    Position = new float3(localTransform.ValueRO.Position),
                    Scale = 1f
                });
                SystemAPI.SetComponent(NewUnit, 
                    new SquadLastPlayerOrder { targetPoz = rallyPointComponent.ValueRO.position });
                SystemAPI.SetComponent(NewUnit, new TargetPosition { value = rallyPointComponent.ValueRO.position });
            }

            progress.ValueRW.production = 0;
            sequenceBuffer.RemoveAt(0);
        }
    }
    
}