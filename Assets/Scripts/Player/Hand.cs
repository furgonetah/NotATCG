using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public List<Card> cardsInHand = new List<Card>();
    public int maxHandSize = 10;
    
    public void AddCard(Card card)
    {
        if (cardsInHand.Count < maxHandSize)
        {
            cardsInHand.Add(card);
            // TODO: UI de añadir carta
            Debug.Log($"Carta añadida a la mano: {card.cardName}");
        }
    }
    
    public void RemoveCard(Card card)
    {
        cardsInHand.Remove(card);
        // TODO: UI de remover
        Debug.Log($"Carta removida de la mano: {card.cardName}");
    }
    
    public void ClearHand()
    {
        cardsInHand.Clear();
    }

    public int GetHandSize()
    {
        return cardsInHand.Count;
    }
    
    public bool IsEmpty()
{
    return cardsInHand.Count == 0;
}
}