using System.Collections.Generic;

public class EnemiesAndFriends
{
    private IReadOnlyDictionary<int, bool> _isMyFriends;

    public EnemiesAndFriends(IReadOnlyDictionary<int, bool> isMyFriends)
    {
        _isMyFriends = isMyFriends;
    }
}