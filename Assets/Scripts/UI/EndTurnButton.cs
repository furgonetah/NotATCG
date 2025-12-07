using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

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
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonGameManager.Instance != null)
        {
            gameState = PhotonGameManager.Instance.gameState;
        }
        else if (GameManager.Instance != null)
        {
            gameState = GameManager.Instance.gameState;
        }

        UpdateButtonState();
    }

    public void UpdateButtonState()
    {
        if (gameState == null || gameState.activePlayer == null)
        {
            button.interactable = false;
            return;
        }

        bool isNetworked = PhotonNetwork.IsConnected && PhotonNetwork.InRoom;

        if (isNetworked && PhotonGameManager.Instance != null)
        {
            Player localPlayer = PhotonGameManager.Instance.localPlayer;
            button.interactable = (gameState.activePlayer == localPlayer);
        }
        else
        {
            button.interactable = true;
        }
    }
}
