using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
[BurstCompile]
public partial struct EntityBufferToLocal : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MyIdComponent>();
        state.RequireForUpdate<LocalOrderEntityBuffer>();
        state.RequireForUpdate<OutputOrderEntityPozLocal>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (localOrderEntityBuffer, outputPoz, outputPozLocal, myId) in SystemAPI
                     .Query<DynamicBuffer<LocalOrderEntityBuffer>, RefRW<OutputOrderEntityPoz>, RefRW<OutputOrderEntityPozLocal>, RefRO<MyIdComponent>>())
        {
            if (outputPoz.ValueRO.size.Equals(outputPozLocal.ValueRO.size) &&
                outputPoz.ValueRO.center.Equals(outputPozLocal.ValueRO.center))
            {
                continue;
            }
            outputPozLocal.ValueRW.size = outputPoz.ValueRO.size;
            outputPozLocal.ValueRW.center = outputPoz.ValueRO.center;
            localOrderEntityBuffer.Clear();
            if (outputPozLocal.ValueRO.center == new Vector3())
            {
                var _entity = Entity.Null;
                float b = 1;
                foreach (var (unit, entity) in SystemAPI.Query<RefRO<LocalTransform>>().WithAny<MovementSpeed,ProductionProgress>()
                             .WithEntityAccess())
                {
                    if(SystemAPI.HasComponent<ArrivalAction>(entity))
                        if(SystemAPI.GetComponent<ArrivalAction>(entity).value == -3)
                            continue;
                
                    var ifSquadEntity = SystemAPI.HasComponent<Parent>(entity) ? SystemAPI.GetComponent<Parent>(entity).Value : entity;
                    float a = math.distance(unit.ValueRO.Position, outputPozLocal.ValueRW.size);

                    if (a < b)
                    {
                        b = a;
                        _entity = ifSquadEntity;
                    }
                }
                if(_entity != Entity.Null)
                    if (SystemAPI.GetComponent<UnitOwnerComponent>(_entity).OwnerId == myId.ValueRO.value)
                    {
                        localOrderEntityBuffer.Add(new LocalOrderEntityBuffer { value = _entity });
                    }
                
                continue;
            }
            bool inBounds;
            var selectionBounds = new Bounds(outputPozLocal.ValueRO.center, outputPozLocal.ValueRO.size);

            foreach (var (localTransform, entity) in SystemAPI.Query<RefRO<LocalTransform>>()
                         .WithEntityAccess().WithAny<MovementSpeed, ProductionProgress>().WithAll<SelectedTag>())
            {
                if (SystemAPI.HasComponent<ArrivalAction>(entity))
                    if (SystemAPI.GetComponent<ArrivalAction>(entity).value == -3)
                        continue;

                inBounds = selectionBounds.Contains(
                    localTransform.ValueRO.Position
                );
                if (!inBounds)
                    continue;
                if (entity == Entity.Null)
                    continue;
                
                var ifSquadEntity = SystemAPI.HasComponent<Parent>(entity)
                    ? SystemAPI.GetComponent<Parent>(entity).Value
                    : entity;
                var unitOwnerId = SystemAPI.GetComponent<UnitOwnerComponent>(ifSquadEntity).OwnerId;

                if (unitOwnerId != myId.ValueRO.value)
                    continue;
                
                localOrderEntityBuffer.Add(new LocalOrderEntityBuffer { value = ifSquadEntity });
                

                // need more
            }
        }
    }
}