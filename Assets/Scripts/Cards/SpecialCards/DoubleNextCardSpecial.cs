using UnityEngine;
using System.Collections.Generic;

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