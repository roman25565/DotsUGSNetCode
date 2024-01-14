using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LobbyRelaySample.UI
{
    /// <summary>
    /// When inside a lobby, this will show information about a player, whether local or remote.
    /// </summary>
    public class InLobbyUserUI : UIPanelBase
    {
        [SerializeField]
        TMP_Text m_DisplayNameText;

        [SerializeField]
        vivox.VivoxUserHandler m_VivoxUserHandler;

        public bool IsAssigned => UserId != null;
        public string UserId { get; set; }
        LocalPlayer m_LocalPlayer;

        public void SetUser(LocalPlayer localPlayer)
        {
            Show();
            m_LocalPlayer = localPlayer;
            UserId = localPlayer.ID.Value;
            SetIsHost(localPlayer.IsHost.Value);
            SetUserStatus(localPlayer.UserStatus.Value);
            SetDisplayName(m_LocalPlayer.DisplayName.Value);
            SubscribeToPlayerUpdates();

            m_VivoxUserHandler.SetId(UserId);
        }

        void SubscribeToPlayerUpdates()
        {
            m_LocalPlayer.DisplayName.onChanged += SetDisplayName;
            m_LocalPlayer.UserStatus.onChanged += SetUserStatus;
            m_LocalPlayer.IsHost.onChanged += SetIsHost;
        }

        void UnsubscribeToPlayerUpdates()
        {
            if (m_LocalPlayer == null)
                return;
            if (m_LocalPlayer.DisplayName?.onChanged != null)
                m_LocalPlayer.DisplayName.onChanged -= SetDisplayName;
            if (m_LocalPlayer.UserStatus?.onChanged != null)
                m_LocalPlayer.UserStatus.onChanged -= SetUserStatus;
            if (m_LocalPlayer.IsHost?.onChanged != null)
                m_LocalPlayer.IsHost.onChanged -= SetIsHost;
        }

        public void ResetUI()
        {
            if (m_LocalPlayer == null)
                return;
            UserId = null;
            SetUserStatus(PlayerStatus.Lobby);
            Hide();
            UnsubscribeToPlayerUpdates();
            m_LocalPlayer = null;
        }

        void SetDisplayName(string displayName)
        {
            m_DisplayNameText.SetText(displayName);
        }

        void SetUserStatus(PlayerStatus statusText)
        {
            Debug.Log("SetUserStatus");
           // m_StatusText.SetText(SetStatusFancy(statusText));
        }

        void SetIsHost(bool isHost)
        {
            Debug.Log("isHost");
            // m_HostIcon.enabled = isHost;
        }
    }
}