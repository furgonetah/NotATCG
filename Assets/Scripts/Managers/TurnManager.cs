using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [Header("References")]
    public CardQueue cardQueue;
    public HandDisplayUI handDisplayUI;

    private GameState gameState;


    public void StartFirstTurn()
    {
        gameState = GameManager.Instance.gameState;
        gameState.ResetTurnData();
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

        handDisplayUI.ClearSelection();

        gameState.SwapActivePlayer();

        gameState.ResetTurnData();

        Debug.Log($"Turno de {gameState.activePlayer.playerName}");

        //TODO: actualización de UI
    }

    //Para carta especial que incrementa el número de cartas a jugar en un turno
    public void ModifyCardsPerTurn(int amount)
    {
        cardQueue.ModifyMaxCardsThisTurn(amount);
    }
}