using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Player 1 UI")]
    public TextMeshProUGUI player1_HP_Text;
    public TextMeshProUGUI player1_Name_Text;
    public HealthBarUI player1_HealthBar;

    [Header("Player 2 UI")]
    public TextMeshProUGUI player2_HP_Text;
    public TextMeshProUGUI player2_Name_Text;
    public HealthBarUI player2_HealthBar;

    [Header("Round Indicators")]
    public RoundIndicatorUI roundIndicator;

    [Header("Turn Info")]
    public TextMeshProUGUI turnInfo_Text;

    private GameState gameState;
    private bool healthBarsInitialized = false;

    void Start()
    {
        gameState = GameManager.Instance.gameState;
        InitializeHealthBars();
        UpdateAllUI();
    }

    void InitializeHealthBars()
    {
        if (player1_HealthBar != null && GameManager.Instance.player1 != null)
        {
            player1_HealthBar.Initialize(GameManager.Instance.player1);
        }

        if (player2_HealthBar != null && GameManager.Instance.player2 != null)
        {
            player2_HealthBar.Initialize(GameManager.Instance.player2);
        }

        healthBarsInitialized = true;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.gameState != null)
        {
            if (gameState == null)
            {
                gameState = GameManager.Instance.gameState;
            }

            // Inicializar barras de vida si no se ha hecho
            if (!healthBarsInitialized)
            {
                InitializeHealthBars();
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