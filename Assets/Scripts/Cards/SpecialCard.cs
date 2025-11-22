using UnityEngine;
using System.Collections.Generic;

public abstract class SpecialCard : Card
{
    // Clase base para cartas especiales
    // Cada carta especial específica heredará de esta y definirá su propio efecto
    
    public override void Play(Player caster, Player target)
    {
        // Aplicar modificadores antes de ejecutar
        ApplyModifiersToCard(caster);
        
        ExecuteSpecialEffect(caster, target);
    }
    
    // Método abstracto que cada carta especial implementará
    protected abstract void ExecuteSpecialEffect(Player caster, Player target);
}

// ==========================================
// EJEMPLO: Carta que roba cartas
// ==========================================
public class DrawCardsSpecial : SpecialCard
{
    [Header("Draw Effect")]
    public int cardsToDraw = 2;
    
    // Valor modificado
    private int modifiedCardsToDraw;
    
    protected override void ExecuteSpecialEffect(Player caster, Player target)
    {
        int finalCards = ModifierApplicationHelper.GetFinalValue(modifiedCardsToDraw, cardsToDraw);
        caster.DrawCards(finalCards);
        Debug.Log($"{cardName}: {caster.playerName} roba {finalCards} cartas");

        // Resetear
        modifiedCardsToDraw = 0;
    }
    
    protected override Dictionary<string, string> GetCardValues()
    {
        Dictionary<string, string> values = new Dictionary<string, string>();
        values["cards"] = cardsToDraw.ToString();
        return values;
    }
    
    protected override void ApplyModifiersToSelf(List<CardModifier> modifiers)
    {
        modifiedCardsToDraw = ModifierApplicationHelper.ApplyMultiplierModifiers(
            cardsToDraw,
            modifiers,
            ModifierType.MultiplyCardDraw,
            cardName,
            "cartas"
        );
    }
}

// ==========================================
// EJEMPLO: Carta que aumenta límite de cartas por turno
// ==========================================
public class IncreaseCardLimitSpecial : SpecialCard
{
    [Header("Card Limit Effect")]
    public int additionalCards = 1;
    
    protected override void ExecuteSpecialEffect(Player caster, Player target)
    {
        GameManager.Instance.turnManager.ModifyCardsPerTurn(additionalCards);
        Debug.Log($"{cardName}: Límite de cartas aumentado en {additionalCards}");
    }
    
    protected override Dictionary<string, string> GetCardValues()
    {
        Dictionary<string, string> values = new Dictionary<string, string>();
        values["cards"] = additionalCards.ToString();
        return values;
    }
}

// ==========================================
// NUEVA: Carta que duplica la siguiente carta
// ==========================================
public class DoubleNextCardSpecial : SpecialCard
{
    [Header("Double Effect")]
    public float multiplier = 2f; // 2x = duplicar
    
    protected override void ExecuteSpecialEffect(Player caster, Player target)
    {
        // Crear modificador que dura 1 carta
        CardModifier doubleModifier = new CardModifier(
            "Duplicar Valores",
            ModifierType.MultiplyAllValues,
            multiplier,
            0,
            1 // Solo afecta a la siguiente carta
        );
        
        doubleModifier.owner = caster;
        
        // Añadir al jugador
        caster.activeModifiers.Add(doubleModifier);
        
        Debug.Log($"{cardName}: La siguiente carta de {caster.playerName} tendrá valores duplicados");
    }
    
    protected override Dictionary<string, string> GetCardValues()
    {
        Dictionary<string, string> values = new Dictionary<string, string>();
        values["multiplier"] = multiplier.ToString("F0"); // "2" en lugar de "2.0"
        return values;
    }
}

// ==========================================
// NUEVA: Carta que añade daño fijo a la siguiente carta de ataque
// ==========================================
public class AddDamageNextCardSpecial : SpecialCard
{
    [Header("Damage Boost")]
    public int bonusDamage = 10;
    
    protected override void ExecuteSpecialEffect(Player caster, Player target)
    {
        CardModifier damageModifier = new CardModifier(
            "Bonus de Daño",
            ModifierType.AddFlatDamage,
            1f,
            bonusDamage,
            1
        );
        
        damageModifier.owner = caster;
        caster.activeModifiers.Add(damageModifier);
        
        Debug.Log($"{cardName}: La siguiente carta de ataque de {caster.playerName} hará +{bonusDamage} daño");
    }
    
    protected override Dictionary<string, string> GetCardValues()
    {
        Dictionary<string, string> values = new Dictionary<string, string>();
        values["damage"] = bonusDamage.ToString();
        return values;
    }
}