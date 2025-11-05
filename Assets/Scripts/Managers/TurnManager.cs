using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [Header("References")]
    public CardQueue cardQueue;
    public HandManager handManager; // CAMBIADO: Ahora usa HandManager en lugar de HandDisplayUI

    private GameState gameState;


    public void StartFirstTurn()
    {
        gameState = GameManager.Instance.gameState;
        gameState.ResetTurnData();
        
        // Actualizar qué mano mostrar
        if (handManager != null)
        {
            handManager.UpdateActiveHand();
        }
        
        Debug.Log($"Turno de {gameState.activePlayer.playerName}");
    }
    
    public void EndTurn()
    {
        Debug.Log("EndTurn llamado");

        cardQueue.ExecuteQueue(gameState.activePlayer, gameState.opponentPlayer);

        if (gameState.playerDiedThisTurn)
        {
            GameManager.Instance.OnPlayerDeath(gameState.deadPlayer);
            return;
        }

        // Limpiar selecciones de todas las manos
        if (handManager != null)
        {
            handManager.ClearAllSelections();
        }

        // Cambiar jugador activo
        gameState.SwapActivePlayer();

        // Resetear datos de turno
        gameState.ResetTurnData();

        // Actualizar qué mano mostrar
        if (handManager != null)
        {
            handManager.UpdateActiveHand();
        }

        Debug.Log($"Turno de {gameState.activePlayer.playerName}");
    }

    //Para carta especial que incrementa el número de cartas a jugar en un turno
    public void ModifyCardsPerTurn(int amount)
    {
        cardQueue.ModifyMaxCardsThisTurn(amount);
    }
}