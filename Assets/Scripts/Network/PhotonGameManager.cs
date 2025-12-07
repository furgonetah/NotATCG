using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

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

        // Master Client baraja y sincroniza mazos
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log($"[MASTER] Barajando mazos...");

            // Barajar ambos mazos
            player1.deck.ShuffleDeck();
            player2.deck.ShuffleDeck();

            // Obtener el orden de las cartas de cada mazo
            string[] p1DeckOrder = GetDeckOrder(player1);
            string[] p2DeckOrder = GetDeckOrder(player2);

            Debug.Log($"[MASTER] Enviando orden de mazos - P1: {p1DeckOrder.Length} cartas, P2: {p2DeckOrder.Length} cartas");
            photonView.RPC("SyncDecksAndDrawRPC", RpcTarget.All, p1DeckOrder, p2DeckOrder);
        }

        gameInitialized = true;
    }

    string[] GetDeckOrder(Player player)
    {
        // Acceder al drawPile del mazo usando reflection
        var drawPileField = typeof(Deck).GetField("drawPile",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (drawPileField != null)
        {
            List<Card> drawPile = (List<Card>)drawPileField.GetValue(player.deck);
            string[] order = new string[drawPile.Count];

            for (int i = 0; i < drawPile.Count; i++)
            {
                order[i] = drawPile[i].cardName;
            }

            return order;
        }

        Debug.LogError("[PhotonGameManager] No se pudo acceder al drawPile del mazo");
        return new string[0];
    }

    [PunRPC]
    void SyncDecksAndDrawRPC(string[] p1DeckOrder, string[] p2DeckOrder)
    {
        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Sincronizando mazos - P1: {p1DeckOrder.Length} cartas, P2: {p2DeckOrder.Length} cartas");

        // Reorganizar mazos según el orden enviado por el Master
        ReorganizeDeck(player1, p1DeckOrder);
        ReorganizeDeck(player2, p2DeckOrder);

        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Mazos sincronizados");

        // Ahora todos roban cartas del mazo sincronizado
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

        // Master inicia el juego (ya no necesitamos sincronizar manos por separado)
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("StartFirstTurnRPC", RpcTarget.All);
        }
    }

    void ReorganizeDeck(Player player, string[] deckOrder)
    {
        var drawPileField = typeof(Deck).GetField("drawPile",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (drawPileField == null)
        {
            Debug.LogError("[PhotonGameManager] No se pudo acceder al campo 'drawPile' del Deck");
            return;
        }

        List<Card> currentDrawPile = (List<Card>)drawPileField.GetValue(player.deck);
        List<Card> newDrawPile = new List<Card>();
        List<Card> remainingCards = new List<Card>(currentDrawPile);

        foreach (string cardName in deckOrder)
        {
            Card card = remainingCards.Find(c => c.cardName == cardName);

            if (card != null)
            {
                newDrawPile.Add(card);
                remainingCards.Remove(card);
            }
            else
            {
                Debug.LogError($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Carta '{cardName}' no encontrada en mazo de {player.playerName}");
            }
        }

        currentDrawPile.Clear();
        currentDrawPile.AddRange(newDrawPile);

        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Mazo de {player.playerName} reorganizado: {currentDrawPile.Count} cartas");
    }

    void SyncInitialHandsOrder()
    {
        // Obtener orden de ambas manos
        string[] p1HandOrder = new string[player1.hand.cardsInHand.Count];
        string[] p2HandOrder = new string[player2.hand.cardsInHand.Count];

        for (int i = 0; i < player1.hand.cardsInHand.Count; i++)
        {
            p1HandOrder[i] = player1.hand.cardsInHand[i].cardName;
        }

        for (int i = 0; i < player2.hand.cardsInHand.Count; i++)
        {
            p2HandOrder[i] = player2.hand.cardsInHand[i].cardName;
        }

        Debug.Log($"[MASTER] Sincronizando orden inicial de manos");
        photonView.RPC("SyncInitialHandsRPC", RpcTarget.All, p1HandOrder, p2HandOrder);
    }

    [PunRPC]
    void SyncInitialHandsRPC(string[] p1HandOrder, string[] p2HandOrder)
    {
        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Recibiendo orden inicial de manos");

        // Reorganizar mano de Player1
        ReorganizeHand(player1, p1HandOrder);

        // Reorganizar mano de Player2
        ReorganizeHand(player2, p2HandOrder);

        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Manos iniciales sincronizadas");

        // Iniciar primer turno después de sincronizar
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("StartFirstTurnRPC", RpcTarget.All);
        }
    }

    void ReorganizeHand(Player player, string[] handOrder)
    {
        List<Card> newHand = new List<Card>();
        List<Card> remainingCards = new List<Card>(player.hand.cardsInHand);

        foreach (string cardName in handOrder)
        {
            Card card = remainingCards.Find(c => c.cardName == cardName);

            if (card != null)
            {
                newHand.Add(card);
                remainingCards.Remove(card); // Remover para manejar duplicados
            }
            else
            {
                Debug.LogError($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Carta '{cardName}' no encontrada en mano de {player.playerName}");
            }
        }

        player.hand.cardsInHand.Clear();
        player.hand.cardsInHand.AddRange(newHand);

        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Mano de {player.playerName} reorganizada: {player.hand.cardsInHand.Count} cartas");
    }

    [PunRPC]
    void StartFirstTurnRPC()
    {
        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Iniciando primer turno");
        gameState.SetPhase(GamePhase.Playing);
        turnManager.StartFirstTurn();

        // Actualizar estado del botón de fin de turno
        if (turnManager.endTurnButton != null)
        {
            turnManager.endTurnButton.UpdateButtonState();
        }
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

        // Actualizar estado del botón de fin de turno
        if (turnManager.endTurnButton != null)
        {
            turnManager.endTurnButton.UpdateButtonState();
        }

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

        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] ¡{winner.playerName} gana la partida!");

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
