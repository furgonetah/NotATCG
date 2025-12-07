using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class BackToLobbyButton : MonoBehaviour
{
    [Header("Referencias")]
    public Button backButton;

    void Start()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackToLobbyClicked);
        }
        else
        {
        }
    }

    void OnBackToLobbyClicked()
    {
        // Desactivar sincronización automática de escenas para evitar conflictos
        PhotonNetwork.AutomaticallySyncScene = false;

        if (PhotonNetwork.IsConnected && PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Joined)
        {
            PhotonNetwork.LeaveRoom();
        }
        else if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
        else
        {
            Debug.Log("[BackToLobbyButton] Ya desconectado de Photon");
        }
        
        SceneManager.LoadScene("Lobby");
    }

    void OnDestroy()
    {
        if (backButton != null)
        {
            backButton.onClick.RemoveListener(OnBackToLobbyClicked);
        }
    }
}
