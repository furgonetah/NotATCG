using UnityEngine;
using System.Collections.Generic;

public class AttackCard : Card
{
    [Header("Attack Properties")]
    public bool isPercentageDamage = false;
    
    [Header("Flat Damage")]
    public int flatDamage = 0;
    
    [Header("Percentage Damage")]
    [Range(0f, 1f)]
    public float percentageDamage = 0f;
    private int modifiedDamage;
    
    public override void Play(Player caster, Player target)
    {
        ApplyModifiersToCard(caster);

        if (isPercentageDamage)
        {
            target.TakeDamagePercentage(percentageDamage);
        }
        else
        {
            int finalDamage = ModifierApplicationHelper.GetFinalValue(modifiedDamage, flatDamage);
            target.TakeDamage(finalDamage);
        }

        modifiedDamage = 0;
    }
    
    protected override Dictionary<string, string> GetCardValues()
    {
        Dictionary<string, string> values = new Dictionary<string, string>();
        
        if (isPercentageDamage)
        {
            values["percentage"] = (percentageDamage * 100).ToString("F0");
        }
        else
        {
            values["damage"] = flatDamage.ToString();
        }
        
        return values;
    }
    
    protected override void ApplyModifiersToSelf(List<CardModifier> modifiers)
    {
        modifiedDamage = ModifierApplicationHelper.ApplyModifiers(
            flatDamage,
            modifiers,
            ModifierType.MultiplyDamage,
            ModifierType.AddFlatDamage,
            cardName,
            "daño"
        );
    }
}