using UnityEngine;
using Photon.Pun;

public class HandManager : MonoBehaviour
{
    [Header("Hand Display References")]
    public HandDisplayUI player1HandDisplay;
    public HandDisplayUI player2HandDisplay;

    [Header("Game References")]
    [SerializeField] private GameManager gameManager;

    void Start()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Current;
        }

        if (!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
        {
            UpdateActiveHand();
        }
    }

    public void UpdateActiveHand()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            Debug.Log("[HandManager] Modo multijugador - HandDisplayUI controla visibilidad");
            return;
        }
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
    }
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
    }
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