using System;
using UnityEngine;

public enum MainMenuState
{
    Main,
    LobbyList,
    Lobby,
}
public class MainMenuUIManager : MonoBehaviour
{
    public MainMenuState LocalMainMenuState { get; private set; }
    
    public Action<MainMenuState> onMainMenuStateChanged;
    
    static MainMenuUIManager m_GameManagerInstance;
    public static MainMenuUIManager Instance
    {
        get
        {
            if (m_GameManagerInstance != null)
                return m_GameManagerInstance;
            m_GameManagerInstance = FindObjectOfType<MainMenuUIManager>();
            return m_GameManagerInstance;
        }
    }
    
    private void Awake()
    {
        LocalMainMenuState = MainMenuState.Main;
    }

    void SetGameState(MainMenuState state)
    {
        LocalMainMenuState = state;

        Debug.Log($"Switching Game State to : {LocalMainMenuState}");

        onMainMenuStateChanged.Invoke(LocalMainMenuState);
    }
    
    public void UIChangeMenuState(MainMenuState state)
    {
        // var isQuittingGame = LocalGameState == MainMenuState.Lobby &&
        //                      m_LocalLobby.LocalLobbyState.Value == LobbyState.InGame;
        //
        // if (isQuittingGame)
        // {
        //     //If we were in-game, make sure we stop by the lobby first
        //     state = MainMenuState.Lobby;
        //     ClientQuitGame();
        // }
        Debug.Log(state);
        SetGameState(state);
    }
}