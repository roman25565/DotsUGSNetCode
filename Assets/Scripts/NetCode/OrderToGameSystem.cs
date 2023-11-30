using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;


[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial struct OrderToGameSystem : ISystem
{
    private EntityQuery query;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NetworkId>();
        state.RequireForUpdate<Order>();
        state.RequireForUpdate<OrderEntityBuffer>();
        
        state.RequireForUpdate<ID>();
        state.RequireForUpdate<OrderLocal>();
        state.RequireForUpdate<OrderEntityBufferLocal>();
        
        query = new EntityQueryBuilder(Allocator.Temp)
            .WithAllRW<Order>().WithAll<OrderEntityBuffer>()
            .Build(state.EntityManager);
        //
        // query.SetChangedVersionFilter(new[] { ComponentType.ReadOnly<Order>(), ComponentType.ReadOnly<OrderEntityBuffer>()});
    }
    
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (order, ghostOwner, orderEntityBuffers) in SystemAPI
                     .Query<RefRO<Order>, RefRO<GhostOwner>, DynamicBuffer<OrderEntityBuffer>>())
        {
            foreach (var (orderLocal, id, bufferEntityLocal) in SystemAPI
                         .Query<RefRW<OrderLocal>, RefRW<ID>, DynamicBuffer<OrderEntityBufferLocal>>())
            {
                if(ghostOwner.ValueRO.NetworkId != id.ValueRO.value)
                    continue;
                if (order.ValueRO.type == orderLocal.ValueRO.type &&
                    order.ValueRO.targetEntity == orderLocal.ValueRO.targetEntity && 
                    order.ValueRO.poz1.Equals(orderLocal.ValueRO.poz1) &&
                    order.ValueRO.poz2.Equals(orderLocal.ValueRO.poz2))
                    continue;
                
                
                orderLocal.ValueRW.type = order.ValueRO.type;
                orderLocal.ValueRW.targetEntity = order.ValueRO.targetEntity;
                orderLocal.ValueRW.poz1 = order.ValueRO.poz1;
                orderLocal.ValueRW.poz2 = order.ValueRO.poz2;
                
                Debug.Log("SSS");
                //need
            }
        }
    }
}