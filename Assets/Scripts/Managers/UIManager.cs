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

    [Header("Turn Info")]
    public TextMeshProUGUI turnInfo_Text;

    private GameState gameState;

    void Start()
    {
        gameState = GameManager.Instance.gameState;
        UpdateAllUI();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.gameState != null)
        {
            if (gameState == null)
            {
                gameState = GameManager.Instance.gameState;
            }
            UpdateAllUI();
        }
    }

    void UpdateAllUI()
    {
        if (gameState.activePlayer == null) return;

        player1_Name_Text.text = GameManager.Instance.player1.playerName;
        player2_Name_Text.text = GameManager.Instance.player2.playerName;

        player1_HP_Text.text = $"HP: {GameManager.Instance.player1.currentHP}/{GameManager.Instance.player1.maxHP}";
        player2_HP_Text.text = $"HP: {GameManager.Instance.player2.currentHP}/{GameManager.Instance.player2.maxHP}";

        turnInfo_Text.text = $"Turno de: {gameState.activePlayer.playerName}";
    }
}