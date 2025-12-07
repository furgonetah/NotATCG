using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Player 1 UI")]
    public TextMeshProUGUI player1_HP_Text;
    public TextMeshProUGUI player1_Name_Text;

    [Header("Player 2 UI")]
    public TextMeshProUGUI player2_HP_Text;
    public TextMeshProUGUI player2_Name_Text;

    [Header("Round Indicators")]
    public RoundIndicatorUI roundIndicator;

    [Header("Turn Info")]
    public TextMeshProUGUI turnInfo_Text;

    private GameState gameState;

    void Start()
    {
        if (GameManager.Current != null)
        {
            gameState = GameManager.Current.gameState;
            UpdateAllUI();
        }
    }

    void Update()
    {
        if (GameManager.Current != null && GameManager.Current.gameState != null)
        {
            if (gameState == null)
            {
                gameState = GameManager.Current.gameState;
            }

            UpdateAllUI();
        }
    }

    void UpdateAllUI()
    {
        if (GameManager.Current == null || gameState == null || gameState.activePlayer == null) return;

        player1_Name_Text.text = GameManager.Current.player1.playerName;
        player2_Name_Text.text = GameManager.Current.player2.playerName;

        player1_HP_Text.text = $"{GameManager.Current.player1.currentHP}";
        player2_HP_Text.text = $"{GameManager.Current.player2.currentHP}";

        turnInfo_Text.text = $"Turno de {gameState.activePlayer.playerName}";
    }
}