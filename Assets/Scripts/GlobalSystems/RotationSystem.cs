using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct RotationSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (stateID,targetPosition,localTransform,entity)//Rotation System
                 in SystemAPI.Query<RefRW<StateID>,RefRO<TargetPosition>,RefRW<LocalTransform>>().WithEntityAccess())
        {
            if (stateID.ValueRO.value != 1 && stateID.ValueRO.value != 4) 
                continue;
            float3 forwardDirection = math.mul(localTransform.ValueRO.Rotation.value, new float3(0, 0, 1)); // Орієнтація сутності
            float targetDegree = math.atan2(targetPosition.ValueRO.value[2] - localTransform.ValueRO.Position[2] , targetPosition.ValueRO.value[0] - localTransform.ValueRO.Position[0]) * 180 /
                               math.PI;
            float forwardDegree = math.atan2(forwardDirection[2] , forwardDirection[0]) * 180 /
                                  math.PI;
            if (targetDegree < 0) 
                targetDegree += 360;
            if (forwardDegree < 0)
                forwardDegree += 360;
            
            var vectorX = targetPosition.ValueRO.value[0] - localTransform.ValueRO.Position[0];
            var vectorZ = targetPosition.ValueRO.value[2] - localTransform.ValueRO.Position[2];
            var norm = math.normalize(new float2(vectorX, vectorZ));
            var arcSize = targetDegree - forwardDegree;
            
            var rightCof = (arcSize > 0 && arcSize < 180) || arcSize < -180 ? -1 : 1;                                                                               //куда повертати;
            if (math.distance(new float2(forwardDirection[0], forwardDirection[2]),norm) < 0.1f)
            { 
                var iii = math.distance(new float2(forwardDirection[0], forwardDirection[2]), norm);
                localTransform.ValueRW.Rotation.value = LocalTransform.FromRotation(localTransform.ValueRO.Rotation).RotateY(rightCof * iii).Rotation.value;
                if (stateID.ValueRO.value == 4)
                {
                    stateID.ValueRW.value = 2;
                }
                else
                {
                    stateID.ValueRW.value = 0;
                }
                if(SystemAPI.HasComponent<Parent>(entity))
                    SystemAPI.SetComponent(entity, new MovementSpeed{value = SystemAPI.GetComponent<SquadStartSpeed>(SystemAPI.GetComponent<Parent>(entity).Value).startSpeed});
                continue;
            }
            localTransform.ValueRW.Rotation.value = LocalTransform.FromRotation(localTransform.ValueRO.Rotation.value).RotateY(rightCof * Time.deltaTime * 8).Rotation.value;
        }
    }
}
