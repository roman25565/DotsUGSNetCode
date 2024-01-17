public static class MyId
{
    private static int _myId;
    public static int Value
    {
        get => _myId;
    }

    public static void SetMyID(int id)
    {
        _myId = id;
    }
}