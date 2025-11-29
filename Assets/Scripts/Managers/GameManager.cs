using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{

    // TODO: todo lo referente a los tiempos hay que corregirlos
    public static GameManager Instance { get; private set; }

    /// <summary>
    /// Devuelve el GameManager activo (GameManager en singleplayer, PhotonGameManager en multijugador)
    /// Usar esta propiedad para acceso unificado en todos los scripts
    /// </summary>
    public static GameManager Current
    {
        get
        {
            // Si estamos en multijugador, usar PhotonGameManager
            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonGameManager.Instance != null)
            {
                return PhotonGameManager.Instance;
            }
            // Caso por defecto: GameManager singleton
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
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Inicializar GameState
        gameState = new GameState();
    }

    void Start()
    {
        InitializeGame();
    }

    void Update()
    {
        // Actualizar timer si está activo
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

        Debug.Log("Juego inicializado.");
        Debug.Log($"Active Player: {gameState.activePlayer?.playerName ?? "NULL"}");
        Debug.Log($"Opponent Player: {gameState.opponentPlayer?.playerName ?? "NULL"}");
    }

    void UpdateTimer()
    {
        currentTime -= Time.deltaTime;
        gameState.timeRemaining = currentTime;

        // Penalización por timeout
        if (currentTime <= 0)
        {
            gameState.activePlayer.TakeDamage(GameConstants.TIMEOUT_DAMAGE_PER_SECOND);
            Debug.Log($"{gameState.activePlayer.playerName} pierde {GameConstants.TIMEOUT_DAMAGE_PER_SECOND} PV por timeout!");
        }
    }

    public virtual void OnPlayerDeath(Player deadPlayer)
    {
        gameState.SetPhase(GamePhase.RoundEnd);

        Player winner = (deadPlayer == player1) ? player2 : player1;
        winner.roundsWon++;

        Debug.Log($"{winner.playerName} gana la ronda {gameState.currentRound}!");

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

        Debug.Log($"Comienza la ronda {gameState.currentRound}");
    }

    void EndGame(Player winner)
    {
        gameState.SetPhase(GamePhase.GameEnd);
        gameState.isTimerActive = false;

        int loserScore = (winner == player1) ? player2.roundsWon : player1.roundsWon;

        Debug.Log($"¡{winner.playerName} gana la partida {winner.roundsWon}-{loserScore}!");

        victoryUI.ShowVictory(winner, winner.roundsWon, loserScore);
    }
    public virtual void SetTimerActive(bool active)
    {
        gameState.isTimerActive = active;
    }
}