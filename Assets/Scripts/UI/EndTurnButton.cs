using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

/// <summary>
/// Controla la disponibilidad del botón de fin de turno.
/// Solo permite que el jugador activo termine su propio turno.
/// </summary>
[RequireComponent(typeof(Button))]
public class EndTurnButton : MonoBehaviour
{
    private Button button;
    private GameState gameState;

    void Awake()
    {
        button = GetComponent<Button>();
    }

    void Start()
    {
        // Obtener GameState del manager apropiado
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonGameManager.Instance != null)
        {
            gameState = PhotonGameManager.Instance.gameState;
        }
        else if (GameManager.Instance != null)
        {
            gameState = GameManager.Instance.gameState;
        }

        // Actualizar estado inicial
        UpdateButtonState();
    }

    /// <summary>
    /// Actualiza el estado del botón según si es el turno del jugador local
    /// </summary>
    public void UpdateButtonState()
    {
        if (gameState == null || gameState.activePlayer == null)
        {
            button.interactable = false;
            return;
        }

        // Verificar si estamos en modo multijugador o single-player
        bool isNetworked = PhotonNetwork.IsConnected && PhotonNetwork.InRoom;

        if (isNetworked && PhotonGameManager.Instance != null)
        {
            // Modo multijugador: solo habilitar si el jugador local es el activo
            Player localPlayer = PhotonGameManager.Instance.localPlayer;
            button.interactable = (gameState.activePlayer == localPlayer);
        }
        else
        {
            // Modo single-player: siempre habilitado (ambos jugadores son locales)
            button.interactable = true;
        }
    }
}
