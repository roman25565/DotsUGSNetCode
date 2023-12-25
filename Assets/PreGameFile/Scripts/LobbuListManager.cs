using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbuListManager : UIPanelBase
{
    [SerializeField] private GameObject lobbyes;
    [SerializeField] private GameObject lobbyPrefab;
    [SerializeField] private LobbyApi lobbyApi;
    
    public void Start()
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
    
    void GameStateChanged(MainMenuState state)
    {
        if (state == MainMenuState.LobbyList)
        {
            RefreshList();
        }
    }

    async void RefreshList()
    {
        var lobbyList = await lobbyApi.LoadLobbyList();
        if (lobbyList.Results == null)
            return;
        foreach (Transform child in lobbyes.transform)
            Destroy(child.gameObject);


        foreach (var lobby in lobbyList.Results)
        {
            var newLobby = Instantiate(lobbyPrefab, lobbyes.transform);
            newLobby.GetComponent<Button>().onClick.AddListener(delegate
            {
                lobbyApi.JoinLobby(lobby.Id);
            });
            
            
            var HostName = newLobby.transform.Find("Host Name").gameObject;
            HostName.GetComponent<TextMeshProUGUI>().text = lobby.Name;
            
            var GameMode = newLobby.transform.Find("Game Mode").gameObject;
            GameMode.GetComponent<TextMeshProUGUI>().text = lobby.Data["GameMode"].Value;
            
            var Map = newLobby.transform.Find("Map").gameObject;
            Map.GetComponent<TextMeshProUGUI>().text = lobby.Data["Map"].Value;
            
            var Players = newLobby.transform.Find("Players").gameObject;
            Players.GetComponent<TextMeshProUGUI>().text = lobby.Players.Count + "/" + lobby.MaxPlayers;
        }
    }
}
