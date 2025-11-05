using UnityEngine;

public abstract class SpecialCard : Card
{
    // Clase base para cartas especiales
    // Cada carta especial específica heredará de esta y definirá su propio efecto
    
    public override void Play(Player caster, Player target)
    {
        ExecuteSpecialEffect(caster, target);
    }
    
    // Método abstracto que cada carta especial implementará
    protected abstract void ExecuteSpecialEffect(Player caster, Player target);
}

// EJEMPLO de carta especial concreta (puedes crear más así):
public class DrawCardsSpecial : SpecialCard
{
    [Header("Draw Effect")]
    public int cardsToDraw = 2;
    
    protected override void ExecuteSpecialEffect(Player caster, Player target)
    {
        caster.DrawCards(cardsToDraw);
        Debug.Log($"{cardName}: {caster.playerName} roba {cardsToDraw} cartas");
    }
}

public class IncreaseCardLimitSpecial : SpecialCard
{
    [Header("Card Limit Effect")]
    public int additionalCards = 1;
    
    protected override void ExecuteSpecialEffect(Player caster, Player target)
    {
        GameManager.Instance.turnManager.ModifyCardsPerTurn(additionalCards);
        Debug.Log($"{cardName}: Límite de cartas aumentado en {additionalCards}");
    }
}