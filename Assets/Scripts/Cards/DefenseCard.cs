using UnityEngine;
using System.Collections.Generic;

public class DefenseCard : Card
{
    [Header("Healing Properties")]
    public bool isPercentageHealing = false;
    
    [Header("Flat Healing")]
    public int flatHealing = 0;
    
    [Header("Percentage Healing")]
    [Range(0f, 1f)]
    public float percentageHealing = 0f;
    
    // Valores modificados temporalmente
    private int modifiedHealing;
    
    public override void Play(Player caster, Player target)
    {
        // Aplicar modificadores activos antes de ejecutar
        ApplyModifiersToCard(caster);

        if (isPercentageHealing)
        {
            caster.HealPercentage(percentageHealing);
            Debug.Log($"{cardName} cura {percentageHealing * 100}% HP a {caster.playerName}");
        }
        else
        {
            int finalHealing = ModifierApplicationHelper.GetFinalValue(modifiedHealing, flatHealing);
            caster.Heal(finalHealing);
            Debug.Log($"{cardName} cura {finalHealing} HP a {caster.playerName}");
        }

        // Resetear valores modificados
        modifiedHealing = 0;
    }
    
    /// <summary>
    /// Retorna los valores de la carta para descripción dinámica
    /// </summary>
    protected override Dictionary<string, string> GetCardValues()
    {
        Dictionary<string, string> values = new Dictionary<string, string>();
        
        if (isPercentageHealing)
        {
            values["percentage"] = (percentageHealing * 100).ToString("F0");
        }
        else
        {
            values["healing"] = flatHealing.ToString();
        }
        
        return values;
    }
    
    /// <summary>
    /// Aplica modificadores a los valores internos de la carta
    /// </summary>
    protected override void ApplyModifiersToSelf(List<CardModifier> modifiers)
    {
        modifiedHealing = ModifierApplicationHelper.ApplyModifiers(
            flatHealing,
            modifiers,
            ModifierType.MultiplyHealing,
            ModifierType.AddFlatHealing,
            cardName,
            "curación"
        );
    }
}