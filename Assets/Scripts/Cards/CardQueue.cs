using System.Collections.Generic;
using UnityEngine;

public class CardQueue : MonoBehaviour
{
    private List<Card> queuedCards = new List<Card>();
    private int maxCardsPerTurn = 2;

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
            //Chequeamos si hay trampas activas que modifiquen el target
            Player finalTarget = CheckTrapsAndModifyTarget(caster, target, card);

            card.Play(caster, finalTarget);

            caster.hand.RemoveCard(card);
        }

        ClearQueue();
    }

    private Player CheckTrapsAndModifyTarget(Player caster, Player target, Card cardBeingPlayed)
    {
        // Iteración como tal de las trampas
        // TODO: TrapManager
        return target;
    }

    public void ModifyMaxCardsThisTurn(int amount)
    {
        maxCardsPerTurn += amount;
        Debug.Log($"Límite de cartas modificado a: {maxCardsPerTurn}");
    }
}