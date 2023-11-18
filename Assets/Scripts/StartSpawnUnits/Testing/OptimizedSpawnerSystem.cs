// using Unity.Collections;
// using Unity.Entities;
// using Unity.Transforms;
// using Unity.Burst;
// using UnityEngine;
//
// [BurstCompile]
// public partial struct OptimizedSpawnerSystem : ISystem
// {
//     public void OnCreate(ref SystemState state) { }
//
//     public void OnDestroy(ref SystemState state) { }
//
//     private int count;
//
//     [BurstCompile]
//     public void OnUpdate(ref SystemState state)
//     {
//         EntityCommandBuffer.ParallelWriter ecb = GetEntityCommandBuffer(ref state);
//         count = 0;
//         // Creates a new instance of the job, assigns the necessary data, and schedules the job in parallel.
//         foreach (var a
//                  in SystemAPI.Query<UnitOwnerComponent>())
//         {
//             count++;
//         }
//
//         Debug.Log($"{count}");
//
//         if(count < 40000)
//         {
//             new ProcessSpawnerJob
//             {
//                 Ecb = ecb
//             }.ScheduleParallel();
//         }
//     }
//
//     private EntityCommandBuffer.ParallelWriter GetEntityCommandBuffer(ref SystemState state)
//     {
//         var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
//         var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
//         return ecb.AsParallelWriter();
//     }
// }
//
// [BurstCompile]
// public partial struct ProcessSpawnerJob : IJobEntity
// {
//     public EntityCommandBuffer.ParallelWriter Ecb;
//
//     // IJobEntity generates a component data query based on the parameters of its `Execute` method.
//     // This example queries for all Spawner components and uses `ref` to specify that the operation
//     // requires read and write access. Unity processes `Execute` for each entity that matches the
//     // component data query.
//     private void Execute([ChunkIndexInQuery] int chunkIndex, ref StartSpawnWorkerPrefabComponent spawner)
//     {
//         // If the next spawn time has passed.
//         
//             // Spawns a new entity and positions it at the spawner.
//             Ecb.Instantiate(chunkIndex, spawner.prefab);
//             Ecb.Instantiate(chunkIndex, spawner.prefab);
//             Ecb.Instantiate(chunkIndex, spawner.prefab);
//             Ecb.Instantiate(chunkIndex, spawner.prefab);
//             Ecb.Instantiate(chunkIndex, spawner.prefab);
//             Ecb.Instantiate(chunkIndex, spawner.prefab);
//             Ecb.Instantiate(chunkIndex, spawner.prefab);
//             Ecb.Instantiate(chunkIndex, spawner.prefab);
//             Ecb.Instantiate(chunkIndex, spawner.prefab);
//             Ecb.Instantiate(chunkIndex, spawner.prefab);
//             Ecb.Instantiate(chunkIndex, spawner.prefab);
//             Ecb.Instantiate(chunkIndex, spawner.prefab);
//             // Resets the next spawn time.
//         
//     }
// }