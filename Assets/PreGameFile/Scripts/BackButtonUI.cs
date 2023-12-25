/// <summary>
/// For navigating the main menu.
/// </summary>
public class BackButtonUI : UIPanelBase
{
    public void ToMenu()
    {
        MainMenuManager.UIChangeMenuState(MainMenuState.Main);
    }
}

