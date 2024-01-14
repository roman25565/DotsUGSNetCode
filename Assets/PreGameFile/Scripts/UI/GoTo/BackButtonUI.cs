using LobbyRelaySample.UI;

namespace LobbyRelaySample
{
    /// <summary>
    /// For navigating the main menu.
    /// </summary>
    public class BackButtonUI : UIPanelBase
    {
        public void ToMenu()
        {
            Manager.UIChangeMenuState(GameState.Menu);
        }
    }
}