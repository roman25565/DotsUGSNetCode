using System.Collections.Generic;
using LobbyRelaySample;
using UnityEngine;

public class CoreDataHandler : MonoBehaviour
{
    public static CoreDataHandler instance;


    private string _gameUID;
    private MapData _mapData;
    
    
    public List<LocalPlayer> localPlayers = new ();
    private bool _isHost;
    
    
    private int _myID;

    public int MyId
    {
        get => _myID;
    }
    public string GameUID => _gameUID;
    public string Scene => _mapData != null ? _mapData.sceneName : null;
    public float MapSize => _mapData.mapSize;
    public bool IsHost => _isHost;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void SetGameUID(MapData d)
    {
        _gameUID = $"{d.sceneName}__{System.Guid.NewGuid().ToString()}";
    }

    public void SetGameUID(string uid)
    {
        _gameUID = uid;
    }

    public void SetMapData(MapData mapData)
    {
        _mapData = mapData;
    }

    public void SetLocalPlayers(List<LocalPlayer> localPlayers)
    { 
        this.localPlayers = localPlayers;
    }
    public void SetIsHost(bool isHost)
    { 
        _isHost = isHost;
    }
    public void SetMyId(int myId)
    { 
        _myID = myId;
        Debug.Log("SetMyId :" + myId);
    }
}