
using Unity.NetCode;

[UnityEngine.Scripting.Preserve]
public class GameBootstrap : ClientServerBootstrap
{
    // Start is called before the first frame update
    public override bool Initialize(string defaultWorldName)
    {
        AutoConnectPort = 7979; // Enabled auto connect
        return base.Initialize(defaultWorldName); // Use the regular bootstrap
    }
}
