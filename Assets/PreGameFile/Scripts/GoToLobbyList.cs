
public class GoToLobbyList : UIPanelBase
{ 
    public void ToJoinMenu()
    {
        MainMenuManager.UIChangeMenuState(MainMenuState.LobbyList);
    }
}
