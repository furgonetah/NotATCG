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
    private int modifiedHealing;
    
    public override void Play(Player caster, Player target)
    {
        ApplyModifiersToCard(caster);

        if (isPercentageHealing)
        {
            caster.HealPercentage(percentageHealing);
        }
        else
        {
            int finalHealing = ModifierApplicationHelper.GetFinalValue(modifiedHealing, flatHealing);
            caster.Heal(finalHealing);
        }

        modifiedHealing = 0;
    }
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