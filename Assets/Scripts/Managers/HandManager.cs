using UnityEngine;

/// <summary>
/// Gestiona las manos de ambos jugadores, mostrando solo la del jugador activo
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
            gameManager = GameManager.Instance;
        }

        // Al inicio, mostrar solo la mano del jugador activo
        UpdateActiveHand();
    }

    /// <summary>
    /// Actualiza qué mano debe mostrarse según el jugador activo
    /// Llamar esto al cambiar de turno
    /// </summary>
    public void UpdateActiveHand()
    {
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