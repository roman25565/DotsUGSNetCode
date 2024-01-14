public class MatchDataStatic
{
    private readonly string _mapName;
    private readonly string _gameMode;

    public string MapName
    {
        get => _mapName;
    }

    public string GameMode
    {
        get => _gameMode;
    }

    public MatchDataStatic(string mapName, string gameMode)
    {
        _mapName = MapName;
        _gameMode = GameMode;
    }
}