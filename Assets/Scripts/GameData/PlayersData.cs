using System.Collections.Generic;

public class PlayersData
{
    private IReadOnlyDictionary<int, PlayerDataStatic> _playersStatics;
    private IReadOnlyDictionary<int, PlayerDataDynamic> _playerDynamics;
    private IReadOnlyDictionary<int, PlayerDataStats> _playerStats;

    public PlayersData(
        IReadOnlyDictionary<int, PlayerDataStatic> playersStatics,
        IReadOnlyDictionary<int, PlayerDataDynamic> playerDynamics, 
        IReadOnlyDictionary<int, PlayerDataStats> playerStats)
    {
        _playersStatics = playersStatics;
        _playerDynamics = playerDynamics;
        _playerStats = playerStats;
    }
    
    public int? SpawnPointHasPlayer(int index)
    {
        foreach (var playerDataStatic in _playersStatics)
        {
            if (playerDataStatic.Value.SpawnPoint == index)
                return playerDataStatic.Value.SpawnPoint;
        }
        return null;
    }
    public Dictionary<int, bool> isMyFriends(int myTeam)
    {
        var result = new Dictionary<int, bool>();
        foreach (var playerDataStatic in _playersStatics)
        {
            bool isIMnyTeam = playerDataStatic.Value.Team == myTeam;
            
            result.Add(playerDataStatic.Value.SpawnPoint,isIMnyTeam);
        }

        return result;
    }

    public int GiveMyTeam(int myId)
    {
        return _playersStatics[myId].Team;
    }
}   