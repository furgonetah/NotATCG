using UnityEngine;
using Photon.Pun;

public class TurnManager : MonoBehaviour
{
    [Header("References")]
    public CardQueue cardQueue;
    public HandManager handManager;
    public EndTurnButton endTurnButton;

    private GameState gameState;
    private PhotonCardQueue photonCardQueue;


    public void StartFirstTurn()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonGameManager.Instance != null)
        {
            gameState = PhotonGameManager.Instance.gameState;
        }
        else if (GameManager.Instance != null)
        {
            gameState = GameManager.Instance.gameState;
        }

        gameState.ResetTurnData();

        if (handManager != null)
        {
            handManager.UpdateActiveHand();
        }

        if (endTurnButton != null)
        {
            endTurnButton.UpdateButtonState();
        }

    }
    
    public void EndTurn()
    {

        bool isNetworked = PhotonNetwork.IsConnected && PhotonNetwork.InRoom;

        if (isNetworked)
        {
            if (photonCardQueue == null)
            {
                photonCardQueue = cardQueue.GetComponent<PhotonCardQueue>();
            }

            if (photonCardQueue != null)
            {
                photonCardQueue.EndTurnNetwork(gameState.activePlayer, gameState.opponentPlayer);
            }
            else
            {
                Debug.LogError("[TurnManager] PhotonCardQueue no encontrado en GameObject de CardQueue!");
            }
        }
        else
        {
            cardQueue.ExecuteQueue(gameState.activePlayer, gameState.opponentPlayer);

            if (gameState.playerDiedThisTurn)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.OnPlayerDeath(gameState.deadPlayer);
                }
                return;
            }

            if (handManager != null)
            {
                handManager.ClearAllSelections();
            }

            gameState.SwapActivePlayer();

            gameState.ResetTurnData();

            if (handManager != null)
            {
                handManager.UpdateActiveHand();
            }

            if (endTurnButton != null)
            {
                endTurnButton.UpdateButtonState();
            }

        }
    }

    public void ModifyCardsPerTurn(int amount)
    {
        cardQueue.ModifyMaxCardsThisTurn(amount);
    }
}