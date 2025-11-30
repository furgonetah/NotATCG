using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PhotonCardQueue : MonoBehaviourPunCallbacks
{
    public CardQueue cardQueue;

    void Awake()
    {
        // photonView ya está disponible automáticamente desde MonoBehaviourPunCallbacks

        // Obtener referencia a CardQueue
        cardQueue = GetComponent<CardQueue>();
        if (cardQueue == null)
        {
            Debug.LogError("[PhotonCardQueue] No se encontró componente CardQueue en el GameObject");
        }
    }

    public void EndTurnNetwork(Player caster, Player target)
    {
        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] EndTurnNetwork llamado");

        // Determinar ID del jugador que lanza
        int casterID = (caster == PhotonGameManager.Instance.player1) ? 1 : 2;

        // PASO 1: Sincronizar orden de la mano del jugador activo primero
        string[] handOrder = new string[caster.hand.cardsInHand.Count];
        for (int i = 0; i < caster.hand.cardsInHand.Count; i++)
        {
            handOrder[i] = caster.hand.cardsInHand[i].cardName;
        }

        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Sincronizando mano de Player{casterID}: {handOrder.Length} cartas");
        photonView.RPC("SyncPlayerHandRPC", RpcTarget.All, casterID, handOrder);

        // PASO 2: Serializar cola como índices de cartas en la mano
        List<int> cardIndices = new List<int>();

        // Usar reflection para acceder a la lista privada queuedCards
        var queuedCardsField = typeof(CardQueue).GetField("queuedCards",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        List<Card> queuedCards = (List<Card>)queuedCardsField.GetValue(cardQueue);

        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Cartas en cola: {queuedCards.Count}");

        foreach (Card card in queuedCards)
        {
            int index = caster.hand.cardsInHand.IndexOf(card);
            cardIndices.Add(index);
            Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Carta '{card.cardName}' en índice {index}");
        }

        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Enviando RPC ExecuteQueueRPC - Caster: Player{casterID}, Cartas: {cardIndices.Count}");

        // PASO 3: Enviar RPC de ejecución
        photonView.RPC("ExecuteQueueRPC", RpcTarget.All, casterID, cardIndices.ToArray());
    }

    [PunRPC]
    void SyncPlayerHandRPC(int playerID, string[] handOrder)
    {
        Player player = (playerID == 1) ? PhotonGameManager.Instance.player1 : PhotonGameManager.Instance.player2;

        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Sincronizando mano de {player.playerName}: {handOrder.Length} cartas");

        List<Card> newHand = new List<Card>();
        List<Card> remainingCards = new List<Card>(player.hand.cardsInHand);

        foreach (string cardName in handOrder)
        {
            // Buscar la carta en las cartas restantes
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

        // Reemplazar la mano
        player.hand.cardsInHand.Clear();
        player.hand.cardsInHand.AddRange(newHand);

        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Mano de {player.playerName} sincronizada: {player.hand.cardsInHand.Count} cartas");
    }

    [PunRPC]
    void ExecuteQueueRPC(int casterID, int[] cardIndices)
    {
        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] ExecuteQueueRPC recibido - Caster: Player{casterID}, Índices: {cardIndices.Length}");

        Player caster = (casterID == 1) ? PhotonGameManager.Instance.player1 : PhotonGameManager.Instance.player2;
        Player target = (casterID == 1) ? PhotonGameManager.Instance.player2 : PhotonGameManager.Instance.player1;

        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Ejecutando cola de {caster.playerName} contra {target.playerName}");

        // Reconstruir cola desde índices
        List<Card> cardsToExecute = new List<Card>();
        foreach (int index in cardIndices)
        {
            if (index >= 0 && index < caster.hand.cardsInHand.Count)
            {
                Card card = caster.hand.cardsInHand[index];
                cardsToExecute.Add(card);
                Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Agregada carta '{card.cardName}' desde índice {index}");
            }
            else
            {
                Debug.LogError($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Índice inválido: {index} (mano tiene {caster.hand.cardsInHand.Count} cartas)");
            }
        }

        // Ejecutar secuencialmente (idéntico en ambos clientes)
        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Ejecutando {cardsToExecute.Count} cartas");
        foreach (Card card in cardsToExecute)
        {
            Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Ejecutando carta: {card.cardName}");
            card.Play(caster, target);
            caster.hand.RemoveCard(card);
        }

        // Limpiar cola local
        cardQueue.ClearQueue();

        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Cola ejecutada y limpiada");

        // Verificar si alguien murió
        if (PhotonGameManager.Instance.gameState.playerDiedThisTurn)
        {
            Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Un jugador ha muerto - {PhotonGameManager.Instance.gameState.deadPlayer.playerName}");

            // Solo el Master Client maneja la muerte
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonGameManager.Instance.OnPlayerDeath(PhotonGameManager.Instance.gameState.deadPlayer);
            }
            return; // No continuar con el siguiente turno
        }

        // Swap active player
        PhotonGameManager.Instance.gameState.SwapActivePlayer();
        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Jugador activo cambiado a: {PhotonGameManager.Instance.gameState.activePlayer.playerName}");

        // Actualizar estado del botón de fin de turno inmediatamente
        if (PhotonGameManager.Instance.turnManager.endTurnButton != null)
        {
            PhotonGameManager.Instance.turnManager.endTurnButton.UpdateButtonState();
        }

        // Solo el master inicia el siguiente turno
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("StartNextTurnRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    void StartNextTurnRPC()
    {
        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] StartNextTurnRPC - Iniciando siguiente turno");
        PhotonGameManager.Instance.turnManager.StartFirstTurn();
    }
}
