using UnityEngine;

public class GameStateVisibilityUI : UIPanelBase
{
    [SerializeField]
    MainMenuState ShowThisWhen;

    void GameStateChanged(MainMenuState state)
    {
        // Debug.Log(state);
        
        Debug.Log(ShowThisWhen);
        Debug.Log(ShowThisWhen.Equals(state));
        
        if (!ShowThisWhen.Equals(state))
            Hide();
        else
            Show();
    }

    public override void Start()
    {
        base.Start();
        MainMenuManager.onMainMenuStateChanged += GameStateChanged;
    }

    void OnDestroy()
    {
        if (MainMenuManager == null)
            return;
        MainMenuManager.onMainMenuStateChanged -= GameStateChanged;
    }
}
