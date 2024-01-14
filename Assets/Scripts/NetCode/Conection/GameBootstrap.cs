using System;
using Unity.Entities;
using Unity.NetCode;

// Create a custom bootstrap, which enables auto-connect.
// The bootstrap can also be used to configure other settings as well as to
// manually decide which worlds (client and server) to create based on user input
[UnityEngine.Scripting.Preserve]
public class GameBootstrap : ClientServerBootstrap
{
    public override bool Initialize(string defaultWorldName)
    {
        //Use 0 to manually connect
        AutoConnectPort = 0;

        if (AutoConnectPort != 0)
        {            
            return base.Initialize(defaultWorldName);
        }
        else
        {
            AutoConnectPort = 0;
            CreateLocalWorld(defaultWorldName);
            return true; ;
        }
    }

}
