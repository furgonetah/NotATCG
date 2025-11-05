using UnityEngine;

public class CardSlot : MonoBehaviour
{
    [Header("Slot Info")]
    public CardVisual cardInSlot;
    
    public bool IsEmpty()
    {
        return cardInSlot == null;
    }
    
    public void SetCard(CardVisual card)
    {
        cardInSlot = card;
    }
    
    public void ClearCard()
    {
        cardInSlot = null;
    }
}