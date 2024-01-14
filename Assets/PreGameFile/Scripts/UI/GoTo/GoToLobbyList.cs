using LobbyRelaySample.UI;

namespace LobbyRelaySample
{
    public class GoToLobbyList : UIPanelBase
    {
        public void ToLobbyList()
        {
            Manager.UIChangeMenuState(GameState.LobbyList);
        }
    }
}