using UnityEngine;
using Photon.Pun;

/// <summary>
/// Gestiona las manos de ambos jugadores
/// - Single-player: Muestra solo la mano del jugador activo
/// - Multijugador: HandDisplayUI se encarga de mostrar solo la mano local
/// </summary>
public class HandManager : MonoBehaviour
{
    [Header("Hand Display References")]
    [SerializeField] private HandDisplayUI player1HandDisplay;
    [SerializeField] private HandDisplayUI player2HandDisplay;

    [Header("Game References")]
    [SerializeField] private GameManager gameManager;

    void Start()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Current;
        }

        // Solo en single-player, mostrar la mano del jugador activo
        // En multijugador, HandDisplayUI se encarga de mostrar solo la mano local
        if (!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
        {
            UpdateActiveHand();
        }
    }

    /// <summary>
    /// Actualiza qué mano debe mostrarse según el jugador activo
    /// Solo se usa en modo single-player
    /// </summary>
    public void UpdateActiveHand()
    {
        // En multijugador, no alternar visibilidad de manos
        // HandDisplayUI ya se encarga de mostrar solo la mano local
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            Debug.Log("[HandManager] Modo multijugador - HandDisplayUI controla visibilidad");
            return;
        }

        // Intentar obtener GameManager si aún no está asignado
        if (gameManager == null)
        {
            gameManager = GameManager.Current;
        }

        if (gameManager == null || gameManager.gameState == null)
        {
            Debug.LogWarning("GameManager o GameState es null en HandManager");
            return;
        }

        Player activePlayer = gameManager.gameState.activePlayer;

        if (activePlayer == gameManager.player1)
        {
            ShowPlayer1Hand();
        }
        else if (activePlayer == gameManager.player2)
        {
            ShowPlayer2Hand();
        }
    }

    /// <summary>
    /// Muestra solo la mano del jugador 1
    /// </summary>
    void ShowPlayer1Hand()
    {
        if (player1HandDisplay != null)
        {
            player1HandDisplay.ShowHand();
        }

        if (player2HandDisplay != null)
        {
            player2HandDisplay.HideHand();
        }

        Debug.Log("Mostrando mano de Player 1");
    }

    /// <summary>
    /// Muestra solo la mano del jugador 2
    /// </summary>
    void ShowPlayer2Hand()
    {
        if (player2HandDisplay != null)
        {
            player2HandDisplay.ShowHand();
        }

        if (player1HandDisplay != null)
        {
            player1HandDisplay.HideHand();
        }

        Debug.Log("Mostrando mano de Player 2");
    }

    /// <summary>
    /// Limpia las selecciones de ambas manos
    /// Útil al cambiar de turno
    /// </summary>
    public void ClearAllSelections()
    {
        if (player1HandDisplay != null)
        {
            player1HandDisplay.ClearSelection();
        }

        if (player2HandDisplay != null)
        {
            player2HandDisplay.ClearSelection();
        }
    }

    /// <summary>
    /// Fuerza refresh de la mano del jugador activo
    /// </summary>
    public void RefreshActiveHand()
    {
        if (gameManager == null || gameManager.gameState == null)
            return;

        Player activePlayer = gameManager.gameState.activePlayer;

        if (activePlayer == gameManager.player1 && player1HandDisplay != null)
        {
            player1HandDisplay.ForceRefresh();
        }
        else if (activePlayer == gameManager.player2 && player2HandDisplay != null)
        {
            player2HandDisplay.ForceRefresh();
        }
    }
}