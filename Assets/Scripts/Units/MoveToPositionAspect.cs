using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct MoveToPositionAspect : IAspect
{
    private readonly RefRW<LocalTransform> _localTransform;
    private readonly RefRO<MovementSpeed> _movementSpeed;
    private readonly RefRW<TargetPosition> _targetPosition;
    private readonly RefRW<StateID> _stateID;
    public void Move(float deltaTime)
    {
        if (_stateID.ValueRO.value != 2)
            return;
        
        if (math.distance(_localTransform.ValueRO.Position, _targetPosition.ValueRO.value) > .5f){
            
            //calculate dir
            float3 direction = math.normalize(_targetPosition.ValueRW.value - _localTransform.ValueRO.Position);
            //Move
            _localTransform.ValueRW.Position += deltaTime * _movementSpeed.ValueRO.value * direction;
        }
        
    }

    public void TestReachedTargetPosition(float3 target)
    {
        // float reachedTargetPosition = .5f;
        // if (math.distance(_localTransform.ValueRO.Position, _targetPosition.ValueRO.value) < reachedTargetPosition)
        
        _targetPosition.ValueRW.value = target;
       
    }
}
