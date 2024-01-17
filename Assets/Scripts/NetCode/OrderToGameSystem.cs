using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;


[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct OrderToGameSystem : ISystem
{
    private EntityQuery query;
    private DynamicBuffer<BuilderPrefabs> builderPrefabs;
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MyIdComponent>();
        state.RequireForUpdate<InputOrder>();
        state.RequireForUpdate<InputOrderEntityPoz>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        builderPrefabs = SystemAPI.GetSingletonBuffer<BuilderPrefabs>();
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        
        foreach (var (order, orderLocal, localOrderEntityBuffer, myIdComponent) in SystemAPI
                     .Query<RefRO<OutputOrder>, RefRW<OrderLocal>,DynamicBuffer<LocalOrderEntityBuffer>,RefRO<MyIdComponent>>())
        {
            if (order.ValueRO.type == orderLocal.ValueRO.type &&
                order.ValueRO.poz1.Equals(orderLocal.ValueRO.poz1) &&
                order.ValueRO.poz2.Equals(orderLocal.ValueRO.poz2))
                continue;

            orderLocal.ValueRW.type = order.ValueRO.type;
            orderLocal.ValueRW.poz1 = order.ValueRO.poz1;
            orderLocal.ValueRW.poz2 = order.ValueRO.poz2;

            var priorityEntity = Entity.Null;
            
            var i = 0;
            foreach (var bufferElement in localOrderEntityBuffer)
            {
                var priority = SystemAPI.GetComponent<UnitPriorityComponent>(bufferElement.value).priority;
                if (priority > i)
                {
                    i = priority;
                    priorityEntity = bufferElement.value;
                }
            }

            if (orderLocal.ValueRO.type is >= 0 and <= 100)
            {
                SystemAPI.SetComponent(priorityEntity, new TargetPosition{value = orderLocal.ValueRO.poz1});
                SystemAPI.SetComponent(priorityEntity, new ArrivalAction{value = -2});
                
                var NewBuilding = ecb.Instantiate(builderPrefabs[orderLocal.ValueRO.type].Value);
                
                ecb.SetComponent(NewBuilding, new UnitOwnerComponent{OwnerId = myIdComponent.ValueRO.value});
                ecb.AddComponent<ConstructionProgress>(NewBuilding);
                ecb.SetComponent(NewBuilding, new ConstructionProgress{progress = -1, myBilder = priorityEntity});
                ecb.SetComponent(NewBuilding, new LocalTransform{Position = orderLocal.ValueRO.poz1,Rotation = new quaternion(0,orderLocal.ValueRO.poz2.x,0,orderLocal.ValueRO.poz2.y),Scale = 1});
            }
            else if(orderLocal.ValueRO.type == 101)
                foreach (var bufferElement in localOrderEntityBuffer)
                {
                    var entity = bufferElement.value;
                    if (SystemAPI.HasComponent<TargetPosition>(entity)) //is Worker or hero
                    {
                        SystemAPI.SetComponent(entity, new StateID { value = 4 });
                        SystemAPI.SetComponent(entity, new TargetPosition { value = orderLocal.ValueRO.poz1 });
                        SystemAPI.SetComponent(entity, new ArrivalAction { value = -1 });
                    }
                    else if (SystemAPI.HasComponent<RallyPointComponent>(entity)) //is Building
                        SystemAPI.SetComponent(entity, new RallyPointComponent { position = orderLocal.ValueRO.poz1 });

                    else if (SystemAPI.HasComponent<SquadMaxHealth>(entity)) //is Squad
                    {
                        Entity targetEntity = Entity.Null;
                        float b = 1;
                        foreach (var (localTransform, entityW) in SystemAPI
                                     .Query<RefRO<LocalTransform>>() //search target entity
                                     .WithAny<MovementSpeed, ProductionProgress>().WithEntityAccess())
                        {
                            var ifSquadEntity = SystemAPI.HasComponent<Parent>(entityW)
                                ? SystemAPI.GetComponent<Parent>(entityW).Value
                                : entityW;
                            float a = math.distance(localTransform.ValueRO.Position, orderLocal.ValueRO.poz1);

                            if (a < b)
                            {
                                b = a;
                                targetEntity = ifSquadEntity;
                            }
                        }

                        if (targetEntity == Entity.Null)
                        {
                            SystemAPI.SetComponent(entity,
                                new SquadLastPlayerOrder
                                    { type = -1, targetPoz = orderLocal.ValueRO.poz1 });

                        }
                        else if (SystemAPI.GetComponent<UnitOwnerComponent>(targetEntity).OwnerId !=
                                 myIdComponent.ValueRO.value) //ворожий айді
                        {
                            Debug.Log("yes");
                            SystemAPI.SetComponent(entity,
                                new SquadLastPlayerOrder { targetEntity = targetEntity, type = -2 });
                            SystemAPI.SetComponent(entity, new ReadyForInitializeCommand { value = 1 });

                        }
                        else //дружній айді
                        {
                        }
                    }
                }
            else if (orderLocal.ValueRO.type >= 300)
            {
                if(priorityEntity == Entity.Null)
                    Debug.Log("Entity Build not found!");
                
                var buffer = SystemAPI.GetBufferLookup<ProductionSequenceBuffer>()[priorityEntity];
                buffer.Add(orderLocal.ValueRO.type - 300);
            }
        }
        state.Dependency.Complete();

        // Now that the job is completed, you can enact the changes.
        // Note that Playback can only be called on the main thread.
        ecb.Playback(state.EntityManager);

        // You are responsible for disposing of any ECB you create.
        ecb.Dispose();
    }
}