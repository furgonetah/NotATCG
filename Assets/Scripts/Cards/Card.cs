using UnityEngine;

public abstract class Card : MonoBehaviour
{
    [Header("Card Info")]
    public string cardName;
    public CardType cardType;
    [TextArea(3, 5)]
    public string cardDescription;
    
    [Header("Visual")]
    public Sprite cardArt;
    
    public abstract void Play(Player caster, Player target);

    public virtual bool CheckTrapCondition(GameState state)
    {
        return false;
    }
    
    public virtual Player ModifyTarget(Player originalCaster, Player originalTarget)
    {
        return originalTarget;
    }
    
    public virtual void OnTrapActivate(Player caster, Player target)
    {
        // Override en TrapCard
    }
}

public enum CardType
{
    Attack,
    Defense,
    Special,
    Trap
}