using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonGameManager : GameManager, IPunObservable, IInRoomCallbacks
{
    public new static PhotonGameManager Instance { get; private set; }

    public Player localPlayer;
    public Player remotePlayer;

    private bool gameInitialized = false;

    // PhotonView para RPCs (se asigna automáticamente al tener PhotonView component)
    private PhotonView photonView;

    void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    void Awake()
    {
        // Singleton para PhotonGameManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Obtener PhotonView
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            Debug.LogError("[PhotonGameManager] PhotonView component no encontrado!");
        }

        // Inicializar GameState
        gameState = new GameState();
    }

    // IPunObservable implementation (requerido para PhotonView)
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // No necesitamos serializar nada por ahora, usamos RPCs
    }

    void Start()
    {
        // Solo iniciar el juego si estamos en red
        if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            InitializeGame();
        }
    }

    void Update()
    {
        // Solo el Master Client actualiza el timer
        if (PhotonNetwork.IsMasterClient && gameState.isTimerActive && gameState.currentPhase == GamePhase.Playing)
        {
            UpdateTimer();
        }
    }

    void InitializeGame()
    {
        if (gameInitialized) return;

        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Inicializando juego...");

        gameState.SetPhase(GamePhase.Setup);

        // Determinar jugador local vs remoto
        if (PhotonNetwork.IsMasterClient)
        {
            localPlayer = player1;
            remotePlayer = player2;
            Debug.Log("[MASTER] Soy Player1 (local), Player2 es remoto");
        }
        else
        {
            localPlayer = player2;
            remotePlayer = player1;
            Debug.Log("[CLIENT] Soy Player2 (local), Player1 es remoto");
        }

        // Master Client genera las semillas y sincroniza
        if (PhotonNetwork.IsMasterClient)
        {
            int seed1 = Random.Range(0, int.MaxValue);
            int seed2 = Random.Range(0, int.MaxValue);
            Debug.Log($"[MASTER] Generando semillas: seed1={seed1}, seed2={seed2}");
            photonView.RPC("SyncShuffleSeeds", RpcTarget.All, seed1, seed2);
        }

        gameInitialized = true;
    }

    [PunRPC]
    void SyncShuffleSeeds(int seed1, int seed2)
    {
        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Sincronizando mazos con semillas: seed1={seed1}, seed2={seed2}");

        // Barajar mazo de Player1 con semilla 1
        Random.InitState(seed1);
        player1.deck.ShuffleDeck();
        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Mazo de Player1 barajado con seed {seed1}");

        // Barajar mazo de Player2 con semilla 2
        Random.InitState(seed2);
        player2.deck.ShuffleDeck();
        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Mazo de Player2 barajado con seed {seed2}");

        // Robar cartas iniciales (idéntico en ambos clientes)
        for (int i = 0; i < GameConstants.INITIAL_DRAW_COUNT; i++)
        {
            player1.DrawCard();
            player2.DrawCard();
        }

        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Cartas iniciales robadas: {GameConstants.INITIAL_DRAW_COUNT} por jugador");

        // Configurar timer
        currentTime = totalGameTime;
        gameState.timeRemaining = currentTime;
        gameState.isTimerActive = true;

        // Configurar jugadores activos
        gameState.activePlayer = player1;
        gameState.opponentPlayer = player2;

        // Master inicia primer turno
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("StartFirstTurnRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    void StartFirstTurnRPC()
    {
        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Iniciando primer turno");
        gameState.SetPhase(GamePhase.Playing);
        turnManager.StartFirstTurn();
    }

    void UpdateTimer()
    {
        currentTime -= Time.deltaTime;
        gameState.timeRemaining = currentTime;

        // Penalización por timeout (solo Master ejecuta)
        if (currentTime <= 0)
        {
            int damage = GameConstants.TIMEOUT_DAMAGE_PER_SECOND;
            photonView.RPC("ApplyTimeoutDamageRPC", RpcTarget.All, damage);
        }
    }

    [PunRPC]
    void ApplyTimeoutDamageRPC(int damage)
    {
        gameState.activePlayer.TakeDamage(damage);
        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] {gameState.activePlayer.playerName} pierde {damage} PV por timeout");
    }

    public override void OnPlayerDeath(Player deadPlayer)
    {
        // Solo Master Client maneja la lógica de muerte
        if (!PhotonNetwork.IsMasterClient) return;

        int deadPlayerID = (deadPlayer == player1) ? 1 : 2;
        photonView.RPC("OnPlayerDeathRPC", RpcTarget.All, deadPlayerID);
    }

    [PunRPC]
    void OnPlayerDeathRPC(int deadPlayerID)
    {
        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Player {deadPlayerID} murió");

        gameState.SetPhase(GamePhase.RoundEnd);

        Player deadPlayer = (deadPlayerID == 1) ? player1 : player2;
        Player winner = (deadPlayerID == 1) ? player2 : player1;
        winner.roundsWon++;

        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] {winner.playerName} gana la ronda {gameState.currentRound}");

        if (winner.roundsWon >= GameConstants.ROUNDS_TO_WIN)
        {
            int winnerID = (winner == player1) ? 1 : 2;
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("EndGameRPC", RpcTarget.All, winnerID);
            }
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("StartNewRoundRPC", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    void StartNewRoundRPC()
    {
        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Iniciando nueva ronda");

        gameState.currentRound++;

        player1.ResetForNewRound();
        player2.ResetForNewRound();

        gameState.SwapActivePlayer();
        gameState.ResetTurnData();

        gameState.SetPhase(GamePhase.Playing);
        turnManager.StartFirstTurn();

        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Comienza la ronda {gameState.currentRound}");
    }

    [PunRPC]
    void EndGameRPC(int winnerID)
    {
        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Fin del juego - Ganador: Player{winnerID}");

        gameState.SetPhase(GamePhase.GameEnd);
        gameState.isTimerActive = false;

        Player winner = (winnerID == 1) ? player1 : player2;
        int loserScore = (winner == player1) ? player2.roundsWon : player1.roundsWon;

        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] ¡{winner.playerName} gana la partida {winner.roundsWon}-{loserScore}!");

        victoryUI.ShowVictory(winner, winner.roundsWon, loserScore);
    }

    #region IInRoomCallbacks Implementation

    public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Oponente se desconectó");

        // Declarar victoria al jugador restante
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("OnOpponentDisconnectedRPC", RpcTarget.All);
        }
    }

    public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) { }
    public void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged) { }
    public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) { }
    public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient) { }

    #endregion

    [PunRPC]
    void OnOpponentDisconnectedRPC()
    {
        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Victoria por desconexión del oponente");

        gameState.SetPhase(GamePhase.GameEnd);
        gameState.isTimerActive = false;

        // Mostrar victoria para el jugador local
        victoryUI.ShowVictory(localPlayer, 2, 0);
    }

    public override void SetTimerActive(bool active)
    {
        gameState.isTimerActive = active;
    }
}
