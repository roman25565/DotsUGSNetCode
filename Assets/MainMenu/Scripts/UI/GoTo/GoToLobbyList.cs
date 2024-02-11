using MainMenu.UI;

namespace MainMenu
{
    public class GoToLobbyList : UIPanelBase
    {
        public void ToLobbyList()
        {
            Manager.UIChangeMenuState(GameState.LobbyList);
        }
    }
}