using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

/// <summary>
/// Maneja el botón de volver al lobby desde GameOnline
/// Se encarga de desconectar de la sala de Photon y cargar la escena del lobby
/// </summary>
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
            Debug.LogWarning("[BackToLobbyButton] No se asignó el botón en el Inspector");
        }
    }

    void OnBackToLobbyClicked()
    {
        Debug.Log("[BackToLobbyButton] Volviendo al lobby...");

        // Desactivar sincronización automática de escenas para evitar conflictos
        PhotonNetwork.AutomaticallySyncScene = false;

        // Solo intentar desconectar si estamos realmente conectados
        // (no en proceso de desconexión)
        if (PhotonNetwork.IsConnected && PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Joined)
        {
            Debug.Log("[BackToLobbyButton] Saliendo de la sala...");
            PhotonNetwork.LeaveRoom();
        }
        else if (PhotonNetwork.IsConnected)
        {
            Debug.Log("[BackToLobbyButton] Desconectando de Photon...");
            PhotonNetwork.Disconnect();
        }
        else
        {
            Debug.Log("[BackToLobbyButton] Ya desconectado de Photon");
        }

        // Cargar la escena del lobby inmediatamente
        // La desconexión continuará en segundo plano si es necesario
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
