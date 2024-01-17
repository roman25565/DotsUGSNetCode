// using Unity.Collections;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct ServerMessageRpcCommand : IRpcCommand
{
    public FixedString64Bytes message;
}
//
// public struct InitializedClient : IComponentData
// {
//
// }
//
// [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
// public partial class ServerSystem : SystemBase
// {
//
//     private ComponentLookup<NetworkId> _clients;
//
//     protected override void OnCreate()
//     {
//         _clients = GetComponentLookup<NetworkId>(true);
//     }
//
//     protected override void OnUpdate()
//     {
//         _clients.Update(this);
//         var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
//         foreach (var (request, command, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<ClientMessageRpcCommand>>().WithEntityAccess())
//         {
//             Debug.Log(command.ValueRO.message + " from client index " + request.ValueRO.SourceConnection.Index + " version " + request.ValueRO.SourceConnection.Version);
//             commandBuffer.DestroyEntity(entity);
//         }
//
//         foreach (var (request, command, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<SpawnUnitRpcCommand>>().WithEntityAccess())
//         {
//             PrefabOrderComponent prefabs;
//             if (SystemAPI.TryGetSingleton<PrefabOrderComponent>(out prefabs) && prefabs.value != null)
//             {
//                 Entity player = commandBuffer.Instantiate(prefabs.value);
//                 commandBuffer.SetComponent(player, new MyIdComponent
//                 {
//                     value = MyId.Value
//                 });
//         
//                 var networkId = _clients[request.ValueRO.SourceConnection];
//                 commandBuffer.SetComponent(player, new GhostOwner()
//                 {
//                     NetworkId = networkId.Value
//                 });
//         
//                 commandBuffer.AppendToBuffer(request.ValueRO.SourceConnection, new LinkedEntityGroup()
//                 {
//                     Value = player
//                 });
//         
//                 commandBuffer.DestroyEntity(entity);
//             }
//         }
//
//         foreach (var (id, entity) in SystemAPI.Query<RefRO<NetworkId>>().WithNone<InitializedClient>().WithEntityAccess())
//         {
//             commandBuffer.AddComponent<InitializedClient>(entity);
//             SendMessageRpc("Client connected with id = " + id.ValueRO.Value, ConnectionManager.serverWorld);
//         }
//         commandBuffer.Playback(EntityManager);
//         commandBuffer.Dispose();
//     }
//
//     public void SendMessageRpc(string text, World world, Entity target = default)
//     {
//         if (world == null || world.IsCreated == false)
//         {
//             return;
//         }
//         var entity = world.EntityManager.CreateEntity(typeof(SendRpcCommandRequest), typeof(ServerMessageRpcCommand));
//         world.EntityManager.SetComponentData(entity, new ServerMessageRpcCommand()
//         {
//             message = text
//         });
//         if (target != Entity.Null)
//         {
//             world.EntityManager.SetComponentData(entity, new SendRpcCommandRequest()
//             {
//                 TargetConnection = target
//             });
//         }
//     }
// }