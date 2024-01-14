// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace Lobby.UI
// {
//     public class LobbuListUI : UIPanelBase
//     {
//         [SerializeField] private GameObject lobbyes;
//         [SerializeField] private GameObject lobbyPrefab;
//         [SerializeField] private LobbyApi lobbyApi;
//
//         
//         public JoinCreateLobbyUI m_JoinCreateLobbyUI;
//         public override void Start()
//         {
//             base.Start();
//             m_JoinCreateLobbyUI.m_OnTabChanged.AddListener(OnTabChanged);
//             Manager.LobbyList.onLobbyListChange += OnLobbyListChanged;
//         }
//
//         void OnDestroy()
//         {
//             if (Manager == null)
//                 return;
//             Manager.LobbyList.onLobbyListChange -= OnLobbyListChanged;
//         }
//
//         void GameStateChanged(MainMenuState state)
//         {
//             if (state == MainMenuState.LobbyList)
//             {
//                 RefreshList();
//             }
//         }
//
//         async void RefreshList()
//         {
//             var lobbyList = await lobbyApi.LoadLobbyList();
//             if (lobbyList.Results == null)
//                 return;
//             foreach (Transform child in lobbyes.transform)
//                 Destroy(child.gameObject);
//
//
//             foreach (var lobby in lobbyList.Results)
//             {
//                 var newLobby = Instantiate(lobbyPrefab, lobbyes.transform);
//
//                 newLobby.GetComponent<LobbyEntryUI>().id = lobby.Id;
//
//
//                 newLobby.GetComponent<LobbyEntryUI>().onLobbyPressed.AddListener(LobbyButtonSelected);
//                 // newLobby.GetComponent<Button>().onClick.AddListener(delegate { lobbyApi.JoinLobby(lobby.Id); });
//
//
//                 var HostName = newLobby.transform.Find("Host Name").gameObject;
//                 HostName.GetComponent<TextMeshProUGUI>().text = lobby.Name;
//
//                 var GameMode = newLobby.transform.Find("Game Mode").gameObject;
//                 GameMode.GetComponent<TextMeshProUGUI>().text = lobby.Data["GameMode"].Value;
//
//                 var Map = newLobby.transform.Find("Map").gameObject;
//                 Map.GetComponent<TextMeshProUGUI>().text = lobby.Data["Map"].Value;
//
//                 var Players = newLobby.transform.Find("Players").gameObject;
//                 Players.GetComponent<TextMeshProUGUI>().text = lobby.Players.Count + "/" + lobby.MaxPlayers;
//             }
//         }
//
//         public void LobbyButtonSelected(string id)
//         {
//             lobbyApi.JoinLobby(id);
//             MainMenuManager.UIChangeMenuState(MainMenuState.Lobby);
//         }
//     }
// }
