using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public partial struct AddComponentToSingleEntitySystemExample : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        var entity = state.EntityManager.CreateEntity();
        state.EntityManager.AddBuffer<PlayerSelectedBuffer>(entity);
        state.EntityManager.AddComponent<PlayerData>(entity);
        SystemAPI.SetComponent(entity, new PlayerData{playerid = 1,gold = 1000});
    }
}

