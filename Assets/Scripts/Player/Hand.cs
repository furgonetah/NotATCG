using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public List<Card> cardsInHand = new List<Card>();
    public int maxHandSize = GameConstants.HAND_MAX_SIZE;
    
    public void AddCard(Card card)
    {
        if (cardsInHand.Count < maxHandSize)
        {
            cardsInHand.Add(card);
        }
    }
    
    public void RemoveCard(Card card)
    {
        cardsInHand.Remove(card);
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