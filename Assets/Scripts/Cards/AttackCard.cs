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
    
    // Valores modificados temporalmente (se calculan antes de Play)
    private int modifiedDamage;
    
    public override void Play(Player caster, Player target)
    {
        // Aplicar modificadores activos antes de ejecutar
        ApplyModifiersToCard(caster);
        
        if (isPercentageDamage)
        {
            target.TakeDamagePercentage(percentageDamage);
            Debug.Log($"{cardName} inflige {percentageDamage * 100}% de daño a {target.playerName}");
        }
        else
        {
            // Usar valor modificado si existe
            int finalDamage = modifiedDamage > 0 ? modifiedDamage : flatDamage;
            target.TakeDamage(finalDamage);
            Debug.Log($"{cardName} inflige {finalDamage} de daño a {target.playerName}");
        }
        
        // Resetear valores modificados
        modifiedDamage = 0;
    }
    
    /// <summary>
    /// Retorna los valores de la carta para descripción dinámica
    /// </summary>
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
    
    /// <summary>
    /// Aplica modificadores a los valores internos de la carta
    /// </summary>
    protected override void ApplyModifiersToSelf(List<CardModifier> modifiers)
    {
        modifiedDamage = flatDamage; // Empezar con valor base
        
        foreach (CardModifier mod in modifiers)
        {
            switch (mod.type)
            {
                case ModifierType.MultiplyAllValues:
                case ModifierType.MultiplyDamage:
                    modifiedDamage = Mathf.RoundToInt(modifiedDamage * mod.multiplier);
                    Debug.Log($"Modificador '{mod.modifierName}' aplicado: daño {flatDamage} → {modifiedDamage}");
                    break;
                    
                case ModifierType.AddFlatDamage:
                    modifiedDamage += mod.flatBonus;
                    Debug.Log($"Modificador '{mod.modifierName}' aplicado: daño +{mod.flatBonus} = {modifiedDamage}");
                    break;
            }
        }
    }
}