// using Unity.Entities;
// using UnityEngine;
//
// [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
// public partial struct SampleCubeInput : ISystem
// {
//     public void OnUpdate(ref SystemState state)
//     {
//         var iad = 0;
//         foreach (var localTransform in SystemAPI.Query<DynamicBuffer<BuilderPrefabs>>().WithEntityAccess())
//         {
//             iad++;
//             Debug.Log($"s {iad}+ {localTransform.Item2}");
//         }
//     }
// }