using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    /// Devuelve el GameManager activo (GameManager en singleplayer, PhotonGameManager en multijugador)
    public static GameManager Current
    {
        get
        {
            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonGameManager.Instance != null)
            {
                return PhotonGameManager.Instance;
            }
            return Instance;
        }
    }

    [Header("Game State")]
    public GameState gameState;

    [Header("Players")]
    public Player player1;
    public Player player2;

    [Header("Managers")]
    public TurnManager turnManager;
    public CardQueue cardQueue;

    [Header("Game Settings")]
    public int maxRounds = GameConstants.MAX_ROUNDS;
    public float totalGameTime = GameConstants.TOTAL_GAME_TIME;
    protected float currentTime;

    [Header("UI")]
    public VictoryUI victoryUI;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        gameState = new GameState();
    }

    void Start()
    {
        InitializeGame();
    }

    void Update()
    {
        if (gameState.isTimerActive && gameState.currentPhase == GamePhase.Playing)
        {
            UpdateTimer();
        }
    }

    void InitializeGame()
    {
        gameState.SetPhase(GamePhase.Setup);

        gameState.activePlayer = player1;
        gameState.opponentPlayer = player2;

        player1.DrawCards(GameConstants.INITIAL_DRAW_COUNT);
        player2.DrawCards(GameConstants.INITIAL_DRAW_COUNT);

        currentTime = totalGameTime;
        gameState.timeRemaining = currentTime;
        gameState.isTimerActive = true;

        gameState.SetPhase(GamePhase.Playing);
        turnManager.StartFirstTurn();
    }

    void UpdateTimer()
    {
        currentTime -= Time.deltaTime;
        gameState.timeRemaining = currentTime;

        if (currentTime <= 0)
        {
            gameState.activePlayer.TakeDamage(GameConstants.TIMEOUT_DAMAGE_PER_SECOND);
        }
    }

    public virtual void OnPlayerDeath(Player deadPlayer)
    {
        gameState.SetPhase(GamePhase.RoundEnd);

        Player winner = (deadPlayer == player1) ? player2 : player1;
        winner.roundsWon++;

        if (winner.roundsWon >= GameConstants.ROUNDS_TO_WIN)
        {
            EndGame(winner);
        }
        else
        {
            StartNewRound();
        }
    }

    void StartNewRound()
    {
        gameState.currentRound++;

        player1.ResetForNewRound();
        player2.ResetForNewRound();

        gameState.SwapActivePlayer();

        gameState.ResetTurnData();

        gameState.SetPhase(GamePhase.Playing);
        turnManager.StartFirstTurn();
    }

    void EndGame(Player winner)
    {
        gameState.SetPhase(GamePhase.GameEnd);
        gameState.isTimerActive = false;

        int loserScore = (winner == player1) ? player2.roundsWon : player1.roundsWon;

        victoryUI.ShowVictory(winner, winner.roundsWon, loserScore);
    }
    public virtual void SetTimerActive(bool active)
    {
        gameState.isTimerActive = active;
    }
}