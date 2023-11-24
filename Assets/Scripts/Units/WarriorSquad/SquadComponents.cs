using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct SquadDirectionOfView : IComponentData
{
    public Vector2 direction;
}

public struct SquadSpeedMaxAcceleration : IComponentData
{
    public float acceleration;
    public float maxSpeed;
}
public struct SquadStartSpeed : IComponentData
{
    public float startSpeed;
}

public struct SquadLastPlayerOrder : IComponentData
{
    public int type;
    public float3 targetPoz;
    public Entity targetEntity;
    public float3 targetDirectionPoz;
}

public struct SquadIndependentState : IComponentData
{
    public int type;
    public Entity targetEntity;
    public Vector2 direction;
}
public struct SquadMaxHealth : IComponentData
{
    public int maxHealth;
}
public struct SquadExperience : IComponentData
{
    public int lvl;
    public float experience;
}
public struct SquadDamage : IComponentData
{
    public float value;
}
public struct SquadAttackRange : IComponentData
{
    public float attackRange;
}
public struct SquadAttackCd : IComponentData
{
    public float value;
}
public struct SquadStartPosition : IComponentData
{
    public float3 value;
}
public struct ReadyForInitializeCommand : IComponentData
{
    public int value;
}

public struct SelectedFormationsId : IComponentData
{
    public int value;
}

public struct ReadySoldiersCount : IComponentData
{
    public int value;
}