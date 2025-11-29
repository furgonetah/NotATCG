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
        if (GameManager.Current != null)
        {
            gameState = GameManager.Current.gameState;
            InitializeHealthBars();
            UpdateAllUI();
        }
    }

    void InitializeHealthBars()
    {
        if (GameManager.Current == null) return;

        if (player1_HealthBar != null && GameManager.Current.player1 != null)
        {
            player1_HealthBar.Initialize(GameManager.Current.player1);
        }

        if (player2_HealthBar != null && GameManager.Current.player2 != null)
        {
            player2_HealthBar.Initialize(GameManager.Current.player2);
        }

        healthBarsInitialized = true;
    }

    void Update()
    {
        if (GameManager.Current != null && GameManager.Current.gameState != null)
        {
            if (gameState == null)
            {
                gameState = GameManager.Current.gameState;
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
        if (GameManager.Current == null || gameState == null || gameState.activePlayer == null) return;

        player1_Name_Text.text = GameManager.Current.player1.playerName;
        player2_Name_Text.text = GameManager.Current.player2.playerName;

        player1_HP_Text.text = $"HP: {GameManager.Current.player1.currentHP}/{GameManager.Current.player1.maxHP}";
        player2_HP_Text.text = $"HP: {GameManager.Current.player2.currentHP}/{GameManager.Current.player2.maxHP}";

        turnInfo_Text.text = $"Turno de: {gameState.activePlayer.playerName}";
    }
}