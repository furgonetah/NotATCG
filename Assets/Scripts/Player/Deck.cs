using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [Header("Deck Configuration")]
    public List<Card> allCards = new List<Card>();
    private List<Card> drawPile = new List<Card>();
    
    void Awake()
    {
        InitializeDeck();
    }
    
    public void InitializeDeck()
    {
        drawPile.Clear();
        drawPile.AddRange(allCards);
        ShuffleDeck();
    }
    
    public void ShuffleDeck()
    {
        for (int i = 0; i < drawPile.Count; i++)
        {
            Card temp = drawPile[i];
            int randomIndex = Random.Range(i, drawPile.Count);
            drawPile[i] = drawPile[randomIndex];
            drawPile[randomIndex] = temp;
        }
    }
    
    public Card DrawCard()
    {
        if (drawPile.Count > 0)
        {
            Card card = drawPile[0];
            drawPile.RemoveAt(0);
            return card;
        }
        
        return null;
    }
    
    public bool IsEmpty()
    {
        return drawPile.Count == 0;
    }

    public int GetDeckSize()
    {
        return drawPile.Count;
    }

    public Card DrawCardByName(string cardName)
    {
        Card card = drawPile.Find(c => c.cardName == cardName);
        if (card != null)
        {
            drawPile.Remove(card);
            return card;
        }
        return null;
    }

    public void ResetDeck()
    {
        drawPile.Clear();
        InitializeDeck();
    }
}