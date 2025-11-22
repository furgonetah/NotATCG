using System.Collections.Generic;
using UnityEngine;

public class CardQueue : MonoBehaviour
{
    private List<Card> queuedCards = new List<Card>();
    private int maxCardsPerTurn = GameConstants.CARDS_PER_TURN;

    public bool CanAddCard()
    {
        return queuedCards.Count < maxCardsPerTurn;
    }

    public void AddCardToQueue(Card card)
    {
        if (CanAddCard())
        {
            queuedCards.Add(card);
            // TODO: feedback visual (highlight, animación, etc.)
        }
    }

    public void RemoveCardFromQueue(Card card)
    {
        queuedCards.Remove(card);
    }

    public void ClearQueue()
    {
        queuedCards.Clear();
    }

    public void ExecuteQueue(Player caster, Player target)
    {
        if (queuedCards.Count == 0)
        {
            Debug.Log("Turno finalizado sin jugar cartas.");
            return;
        }

        foreach (Card card in queuedCards)
        {
            card.Play(caster, target);
            caster.hand.RemoveCard(card);
        }

        ClearQueue();
    }

    public void ModifyMaxCardsThisTurn(int amount)
    {
        maxCardsPerTurn += amount;
        Debug.Log($"Límite de cartas modificado a: {maxCardsPerTurn}");
    }
}