using System;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;
    private PlayersData _playersData;
    private EnemiesAndFriends _enemiesAndFriends;
    
    private bool _imHost;
    private IReadOnlyDictionary<int, PlayerDataStatic> _playersStatics;
    
    private static int _myId;
    public static int MyId
    {
        get => _myId;
    }

    public bool ImHost
    {
        get => _imHost;
    }
    
    #region Init
    private void Awake()
    {
        Debug.Log("Awake");
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        _myId = SearchID();
        _imHost = SearchImHost();
        _playersData = CreatePlayersData();
        // _enemiesAndFriends = CreateEnemiesAndFriends();
        
        Instance = this;
    }
    private PlayersData CreatePlayersData()
    {
        var a = new MatchDataStatic(mapName: ".",gameMode: ".");
        
        Dictionary<int, PlayerDataStatic> localPlayersStatics = new ();
        Dictionary<int, PlayerDataDynamic> localPlayerDynamics = new ();
        Dictionary<int, PlayerDataStats> localPlayerStats = new ();
        
#if UNITY_EDITOR
        if (CoreDataHandler.instance == null)
        {
            localPlayersStatics.Add(1,new PlayerDataStatic(id: ".",color: Color.blue, spawnPoint: 1,team: 0));
            return new PlayersData(
                playersStatics: localPlayersStatics,
                playerDynamics: localPlayerDynamics,
                playerStats: localPlayerStats);
        }
#endif
        foreach (var player in CoreDataHandler.instance.localPlayers)
        {
            var id = player.Index.Value;
            
            // try { id = int.Parse(player.ID.Value); }
            // catch (FormatException e) { Console.WriteLine(e.Message); }
            
            localPlayersStatics.Add(id, new PlayerDataStatic(id: player.ID.Value, color: Color.red, spawnPoint: player.Index.Value,team: player.Team.Value)); 
            localPlayerDynamics.Add(id, new PlayerDataDynamic(gold: 1000, globalSkills: 0));
            localPlayerStats.Add(id, new PlayerDataStats());//Need
            
            new MatchDataStatic(mapName: "Test", gameMode: "FFA");//Need
        }
        return new PlayersData(
            playersStatics: localPlayersStatics, 
            playerDynamics: localPlayerDynamics, 
            playerStats: localPlayerStats);
    }
    private int SearchID()
    {
#if UNITY_EDITOR
        if (CoreDataHandler.instance == null)
            return 1;
#endif
        return CoreDataHandler.instance.MyId;
    }
    private bool SearchImHost()
    {
#if UNITY_EDITOR
        if (CoreDataHandler.instance == null || CoreDataHandler.instance.localPlayers.Count == 0)
            return true;
#endif
        return CoreDataHandler.instance.IsHost;
    }
    private EnemiesAndFriends CreateEnemiesAndFriends()
    {
        var myTeam = _playersData.GiveMyTeam(MyId);
        var result = _playersData.isMyFriends(myTeam);
        return new EnemiesAndFriends(result);
    }
    #endregion

    public int? SpawnPointHasPlayer(int index)
    {
        return _playersData.SpawnPointHasPlayer(index);
    }
}