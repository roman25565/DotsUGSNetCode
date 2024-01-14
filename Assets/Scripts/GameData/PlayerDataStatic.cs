using UnityEngine;

public class PlayerDataStatic
{
    private readonly string _id;
    private readonly Color _color;
    private readonly int _spawnPoint;
    private readonly int _team;

    public string Id
    {
        get => _id;
    }

    public int SpawnPoint
    {
        get => _spawnPoint;
    }

    public int Team
    {
        get => _team;
    }

    public PlayerDataStatic(string id,Color color, int spawnPoint, int team)
    {
        _id = id;
        _color = color;
        _spawnPoint = spawnPoint;
        _team = team;
    }
    
    //HERO
    //RACE
}