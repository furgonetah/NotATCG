using UnityEngine;
using Photon.Pun;

public class TurnManager : MonoBehaviour
{
    [Header("References")]
    public CardQueue cardQueue;
    public HandManager handManager; // CAMBIADO: Ahora usa HandManager en lugar de HandDisplayUI

    private GameState gameState;
    private PhotonCardQueue photonCardQueue;


    public void StartFirstTurn()
    {
        // Obtener GameState del manager apropiado (PhotonGameManager si estamos en red)
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonGameManager.Instance != null)
        {
            gameState = PhotonGameManager.Instance.gameState;
        }
        else if (GameManager.Instance != null)
        {
            gameState = GameManager.Instance.gameState;
        }

        gameState.ResetTurnData();

        // Actualizar qué mano mostrar
        if (handManager != null)
        {
            handManager.UpdateActiveHand();
        }

        Debug.Log($"[TurnManager] Turno de {gameState.activePlayer.playerName}");
    }
    
    public void EndTurn()
    {
        Debug.Log("[TurnManager] EndTurn llamado");

        // Verificar si estamos en modo multijugador
        bool isNetworked = PhotonNetwork.IsConnected && PhotonNetwork.InRoom;

        if (isNetworked)
        {
            // Modo multijugador: Usar PhotonCardQueue
            if (photonCardQueue == null)
            {
                // Buscar PhotonCardQueue en el GameObject que tiene CardQueue
                photonCardQueue = cardQueue.GetComponent<PhotonCardQueue>();
            }

            if (photonCardQueue != null)
            {
                Debug.Log("[TurnManager] Usando PhotonCardQueue para sincronización en red");
                photonCardQueue.EndTurnNetwork(gameState.activePlayer, gameState.opponentPlayer);
            }
            else
            {
                Debug.LogError("[TurnManager] PhotonCardQueue no encontrado en GameObject de CardQueue!");
            }
        }
        else
        {
            // Modo single-player: Usar CardQueue normal
            Debug.Log("[TurnManager] Usando CardQueue local (single-player)");
            cardQueue.ExecuteQueue(gameState.activePlayer, gameState.opponentPlayer);

            if (gameState.playerDiedThisTurn)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.OnPlayerDeath(gameState.deadPlayer);
                }
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

            Debug.Log($"[TurnManager] Turno de {gameState.activePlayer.playerName}");
        }
    }

    //Para carta especial que incrementa el número de cartas a jugar en un turno
    public void ModifyCardsPerTurn(int amount)
    {
        cardQueue.ModifyMaxCardsThisTurn(amount);
    }
}