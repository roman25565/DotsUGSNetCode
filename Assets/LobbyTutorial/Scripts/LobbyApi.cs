// using System;
// using System.Collections.Generic;
// using Unity.Services.Authentication;
// using Unity.Services.Core;
// using Unity.Services.Lobbies;
// using Unity.Services.Lobbies.Models;
// using UnityEngine;
// using System.Threading.Tasks;
//
//
// using Random = UnityEngine.Random;
//
// namespace Lobby
// {
//     public class LobbyApi : IDisposable
//     {
//         private Lobby hostLobby;
//         private Lobby.LocalLobby joinedLobby;
//         
//         public Lobby CurrentLobby => m_CurrentLobby;
//         Lobby m_CurrentLobby;
//         
//         private float heartbeatTimer;
//         private float lobbyUpdateTimer;
//         private string playerName;
//
//         LobbyEventCallbacks m_LobbyEventCallbacks = new LobbyEventCallbacks();
//
//         async void Awake()
//         {
//             try
//             {
//                 await UnityServices.InitializeAsync();
//             }
//             catch (Exception e)
//             {
//                 Debug.LogException(e);
//             }
//
//             AuthenticationService.Instance.SignedIn += () =>
//             {
//                 Debug.Log("Signed In " + AuthenticationService.Instance.PlayerId);
//             };
//             try
//             {
//                 await AuthenticationService.Instance.SignInAnonymouslyAsync();
//                 Debug.Log("Sign in anonymously succeeded!");
//
//                 // Shows how to get the playerID
//                 Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
//             }
//             catch (AuthenticationException ex)
//             {
//                 // Compare error code to AuthenticationErrorCodes
//                 // Notify the player with the proper error message
//                 Debug.LogException(ex);
//             }
//             catch (RequestFailedException ex)
//             {
//                 // Compare error code to CommonErrorCodes
//                 // Notify the player with the proper error message
//                 Debug.LogException(ex);
//             }
//
//             playerName = "Roman" + Random.Range(10, 99);
//             Debug.Log("UName: " + playerName);
//         }
//
//         private void Update()
//         {
//             HandleLobbyHeartbeat();
//             HandleLobbyPollForUpdates();
//         }
//
//         private async void HandleLobbyHeartbeat()
//         {
//             if (hostLobby != null)
//             {
//                 heartbeatTimer -= Time.deltaTime;
//                 if (heartbeatTimer <= 0f)
//                 {
//                     float heartbeatTimerMax = 15;
//                     heartbeatTimer = heartbeatTimerMax;
//
//                     await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
//                 }
//             }
//         }
//
//         private async void HandleLobbyPollForUpdates()
//         {
//             if (joinedLobby != null)
//             {
//                 lobbyUpdateTimer -= Time.deltaTime;
//                 if (lobbyUpdateTimer <= 0f)
//                 {
//                     float lobbyUpdateTimerMax = 1.1f;
//                     lobbyUpdateTimer = lobbyUpdateTimerMax;
//
//                     Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
//                     joinedLobby = lobby;
//                 }
//             }
//         }
//
//         public async void CreateLobby()
//         {
//             try
//             {
//                 string lobbyName = "new lobby";
//                 int maxPlayers = 4;
//                 CreateLobbyOptions options = new CreateLobbyOptions
//                 {
//                     IsPrivate = false,
//                     Player = GetPlayer(),
//                     Data = new Dictionary<string, DataObject>
//                     {
//                         {
//                             "GameMode",
//                             new DataObject(DataObject.VisibilityOptions.Public, "FFA", DataObject.IndexOptions.S1)
//                         },
//                         {
//                             "Map",
//                             new DataObject(DataObject.VisibilityOptions.Public, "Dast", DataObject.IndexOptions.S2)
//                         }
//                     }
//                 };
//
//                 Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
//
//                 hostLobby = lobby;
//                 joinedLobby = lobby;
//
//                 PrintPlayers(hostLobby);
//
//                 Debug.Log("Created Lobby! " + lobbyName + " " + lobby.MaxPlayers);
//             }
//             catch (Exception e)
//             {
//                 Debug.Log(e);
//             }
//         }
//
//         public async Task<QueryResponse> LoadLobbyList()
//         {
//             try
//             {
//                 // new QueryFilter(QueryFilter.FieldOptions.S1, "FFA",
//                 //     QueryFilter.OpOptions.EQ) //S1 лінія прослуховування FFA - value EQ має дорівнювати
//                 QueryLobbiesOptions options = new QueryLobbiesOptions();
//                 options.Count = 25;
//
//                 // Filter for open lobbies only
//                 options.Filters = new List<QueryFilter>
//                 {
//                     new QueryFilter(
//                         field: QueryFilter.FieldOptions.AvailableSlots,
//                         op: QueryFilter.OpOptions.GT,
//                         value: "0")
//                 };
//
//                 // Order by newest lobbies first
//                 options.Order = new List<QueryOrder>
//                 {
//                     new QueryOrder(
//                         asc: false,
//                         field: QueryOrder.FieldOptions.Created)
//                 };
//
//                 QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(options);
//                 Debug.Log(queryResponse.Results[0].Id);
//
//                 return queryResponse;
//                 // Debug.Log("Lobbies found: " + queryResponse.Results.Count);
//                 // foreach (Lobby lobby in queryResponse.Results)
//                 // {
//                 //     Debug.Log(lobby.Name + " " + lobby.MaxPlayers + " " + lobby.Data["GameMode"].Value);
//                 // }
//             }
//             catch (LobbyServiceException e)
//             {
//                 Debug.Log(e);
//                 return null;
//             }
//         }
//
//         public async void JoinLobby(string lobbyId)
//         {
//             JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
//             {
//                 Player = GetPlayer()
//             };
//
//             // QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(); пошук всіх лоббі
//             Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId, joinLobbyByIdOptions);
//             joinedLobby = lobby;
//             Debug.Log("Joined Lobby with id " + lobbyId);
//
//             PrintPlayers(lobby);
//         }
//
//         private void QuickJoinLobby()
//         {
//             LobbyService.Instance.QuickJoinLobbyAsync();
//         }
//
//         private Player GetPlayer()
//         {
//             return new Player
//             {
//                 Data = new Dictionary<string, PlayerDataObject>
//                 {
//                     { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
//                 }
//             };
//         }
//
//         private void PrintPlayers()
//         {
//             PrintPlayers(joinedLobby);
//         }
//
//         private void PrintPlayers(Lobby lobby)
//         {
//             Debug.Log("Players in lobby " + lobby.Name + " " + lobby.Data["GameMode"].Value + " " +
//                       lobby.Data["Map"].Value);
//             foreach (Player player in lobby.Players)
//             {
//                 Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);
//             }
//         }
//
//         private async void UpdateLobbyGameMode(string gamemode)
//         {
//             try
//             {
//                 hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
//                 {
//                     Data = new Dictionary<string, DataObject>
//                     {
//                         { "GameMode", new DataObject(DataObject.VisibilityOptions.Public, gamemode) }
//                     }
//                 });
//                 joinedLobby = hostLobby;
//             }
//             catch (LobbyServiceException e)
//             {
//                 Debug.Log(e);
//             }
//         }
//
//         private async void UpdatePlayerName(string newPlayerName)
//         {
//             try
//             {
//                 playerName = newPlayerName;
//                 await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId,
//                     new UpdatePlayerOptions
//                     {
//                         Data = new Dictionary<string, PlayerDataObject>
//                         {
//                             {
//                                 "PlayerName",
//                                 new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)
//                             }
//                         }
//                     });
//             }
//             catch (LobbyServiceException e)
//             {
//                 Debug.Log(e);
//             }
//         }
//
//         private async void LeaveLobby()
//         {
//             try
//             {
//                 await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
//             }
//             catch (LobbyServiceException e)
//             {
//                 Debug.Log(e);
//             }
//         }
//
//         private async void KickPlayer()
//         {
//             try
//             {
//                 await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, joinedLobby.Players[1].Id);
//             }
//             catch (LobbyServiceException e)
//             {
//                 Debug.Log(e);
//             }
//         }
//
//         private async void MigrateLobbyHost()
//         {
//             try
//             {
//                 hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
//                 {
//                     HostId = joinedLobby.Players[1].Id
//                 });
//                 joinedLobby = hostLobby;
//
//                 PrintPlayers();
//             }
//             catch (LobbyServiceException e)
//             {
//                 Debug.Log(e);
//             }
//         }
//
//         private async void DeleteLobby()
//         {
//             try
//             {
//                 await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
//             }
//             catch (LobbyServiceException e)
//             {
//                 Debug.Log(e);
//             }
//         }
//
//         #region LobbyEvents
//
//         public async Task BindLocalLobbyToRemote(string lobbyID, LocalLobby localLobby)
//         {
//             m_LobbyEventCallbacks.LobbyDeleted += async () => { await LeaveLobbyAsync(); };
//
//             m_LobbyEventCallbacks.DataChanged += changes =>
//             {
//                 foreach (var change in changes)
//                 {
//                     var changedValue = change.Value;
//                     var changedKey = change.Key;
//
//                     if (changedKey == key_RelayCode)
//                         localLobby.RelayCode.Value = changedValue.Value.Value;
//
//                     if (changedKey == key_LobbyState)
//                         localLobby.LocalLobbyState.Value = (LobbyState)int.Parse(changedValue.Value.Value);
//
//                     if (changedKey == key_LobbyColor)
//                         localLobby.LocalLobbyColor.Value = (LobbyColor)int.Parse(changedValue.Value.Value);
//                 }
//             };
//
//             m_LobbyEventCallbacks.DataAdded += changes =>
//             {
//                 foreach (var change in changes)
//                 {
//                     var changedValue = change.Value;
//                     var changedKey = change.Key;
//
//                     if (changedKey == key_RelayCode)
//                         localLobby.RelayCode.Value = changedValue.Value.Value;
//
//                     if (changedKey == key_LobbyState)
//                         localLobby.LocalLobbyState.Value = (LobbyState)int.Parse(changedValue.Value.Value);
//
//                     if (changedKey == key_LobbyColor)
//                         localLobby.LocalLobbyColor.Value = (LobbyColor)int.Parse(changedValue.Value.Value);
//                 }
//             };
//
//             m_LobbyEventCallbacks.DataRemoved += changes =>
//             {
//                 foreach (var change in changes)
//                 {
//                     var changedKey = change.Key;
//                     if (changedKey == key_RelayCode)
//                         localLobby.RelayCode.Value = "";
//                 }
//             };
//
//             m_LobbyEventCallbacks.PlayerLeft += players =>
//             {
//                 foreach (var leftPlayerIndex in players)
//                 {
//                     localLobby.RemovePlayer(leftPlayerIndex);
//                 }
//             };
//
//             m_LobbyEventCallbacks.PlayerJoined += players =>
//             {
//                 foreach (var playerChanges in players)
//                 {
//                     Player joinedPlayer = playerChanges.Player;
//
//                     var id = joinedPlayer.Id;
//                     var index = playerChanges.PlayerIndex;
//                     var isHost = localLobby.HostID.Value == id;
//
//                     var newPlayer = new LocalPlayer(id, index, isHost);
//
//                     foreach (var dataEntry in joinedPlayer.Data)
//                     {
//                         var dataObject = dataEntry.Value;
//                         ParseCustomPlayerData(newPlayer, dataEntry.Key, dataObject.Value);
//                     }
//
//                     localLobby.AddPlayer(index, newPlayer);
//                 }
//             };
//
//             m_LobbyEventCallbacks.PlayerDataChanged += changes =>
//             {
//                 foreach (var lobbyPlayerChanges in changes)
//                 {
//                     var playerIndex = lobbyPlayerChanges.Key;
//                     var localPlayer = localLobby.GetLocalPlayer(playerIndex);
//                     if (localPlayer == null)
//                         continue;
//                     var playerChanges = lobbyPlayerChanges.Value;
//
//                     //There are changes on the Player
//                     foreach (var playerChange in playerChanges)
//                     {
//                         var changedValue = playerChange.Value;
//
//                         //There are changes on some of the changes in the player list of changes
//                         var playerDataObject = changedValue.Value;
//                         ParseCustomPlayerData(localPlayer, playerChange.Key, playerDataObject.Value);
//                     }
//                 }
//             };
//
//             m_LobbyEventCallbacks.PlayerDataAdded += changes =>
//             {
//                 foreach (var lobbyPlayerChanges in changes)
//                 {
//                     var playerIndex = lobbyPlayerChanges.Key;
//                     var localPlayer = localLobby.GetLocalPlayer(playerIndex);
//                     if (localPlayer == null)
//                         continue;
//                     var playerChanges = lobbyPlayerChanges.Value;
//
//                     //There are changes on the Player
//                     foreach (var playerChange in playerChanges)
//                     {
//                         var changedValue = playerChange.Value;
//
//                         //There are changes on some of the changes in the player list of changes
//                         var playerDataObject = changedValue.Value;
//                         ParseCustomPlayerData(localPlayer, playerChange.Key, playerDataObject.Value);
//                     }
//                 }
//             };
//
//             m_LobbyEventCallbacks.PlayerDataRemoved += changes =>
//             {
//                 foreach (var lobbyPlayerChanges in changes)
//                 {
//                     var playerIndex = lobbyPlayerChanges.Key;
//                     var localPlayer = localLobby.GetLocalPlayer(playerIndex);
//                     if (localPlayer == null)
//                         continue;
//                     var playerChanges = lobbyPlayerChanges.Value;
//
//                     //There are changes on the Player
//                     if (playerChanges == null)
//                         continue;
//
//                     foreach (var playerChange in playerChanges.Values)
//                     {
//                         //There are changes on some of the changes in the player list of changes
//                         Debug.LogWarning("This Sample does not remove Player Values currently.");
//                     }
//                 }
//             };
//
//             m_LobbyEventCallbacks.LobbyChanged += async changes =>
//             {
//                 //Lobby Fields
//                 if (changes.Name.Changed)
//                     localLobby.LobbyName.Value = changes.Name.Value;
//                 if (changes.HostId.Changed)
//                     localLobby.HostID.Value = changes.HostId.Value;
//                 if (changes.IsPrivate.Changed)
//                     localLobby.Private.Value = changes.IsPrivate.Value;
//                 if (changes.IsLocked.Changed)
//                     localLobby.Locked.Value = changes.IsLocked.Value;
//                 if (changes.AvailableSlots.Changed)
//                     localLobby.AvailableSlots.Value = changes.AvailableSlots.Value;
//                 if (changes.MaxPlayers.Changed)
//                     localLobby.MaxPlayerCount.Value = changes.MaxPlayers.Value;
//
//                 if (changes.LastUpdated.Changed)
//                     localLobby.LastUpdated.Value = changes.LastUpdated.Value.ToFileTimeUtc();
//
//                 //Custom Lobby Fields
//
//                 if (changes.PlayerData.Changed)
//                     PlayerDataChanged();
//
//                 void PlayerDataChanged()
//                 {
//                     foreach (var lobbyPlayerChanges in changes.PlayerData.Value)
//                     {
//                         var playerIndex = lobbyPlayerChanges.Key;
//                         var localPlayer = localLobby.GetLocalPlayer(playerIndex);
//                         if (localPlayer == null)
//                             continue;
//                         var playerChanges = lobbyPlayerChanges.Value;
//                         if (playerChanges.ConnectionInfoChanged.Changed)
//                         {
//                             var connectionInfo = playerChanges.ConnectionInfoChanged.Value;
//                             Debug.Log(
//                                 $"ConnectionInfo for player {playerIndex} changed to {connectionInfo}");
//                         }
//
//                         if (playerChanges.LastUpdatedChanged.Changed)
//                         {
//                         }
//                     }
//                 }
//             };
//             m_LobbyEventCallbacks.LobbyEventConnectionStateChanged += lobbyEventConnectionState =>
//             {
//                 Debug.Log($"Lobby ConnectionState Changed to {lobbyEventConnectionState}");
//             };
//
//             m_LobbyEventCallbacks.KickedFromLobby += () =>
//             {
//                 Debug.Log("Left Lobby");
//                 Dispose();
//             };
//             await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobbyID, m_LobbyEventCallbacks);
//         }
//
//         #endregion
//         
//         public void Dispose()
//         {
//             m_CurrentLobby = null;
//             m_LobbyEventCallbacks = new LobbyEventCallbacks();
//         }
//     }
// }