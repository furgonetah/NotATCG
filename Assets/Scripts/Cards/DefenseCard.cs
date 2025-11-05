using UnityEngine;

public class DefenseCard : Card
{
    [Header("Healing Properties")]
    public bool isPercentageHealing = false;
    
    [Header("Flat Healing")]
    public int flatHealing = 0;
    
    [Header("Percentage Healing")]
    [Range(0f, 1f)]
    public float percentageHealing = 0f; 
    
    public override void Play(Player caster, Player target)
    {
        if (isPercentageHealing)
        {
            target.HealPercentage(percentageHealing);
            Debug.Log($"{cardName} cura {percentageHealing * 100}% HP a {target.playerName}");
        }
        else
        {
            target.Heal(flatHealing);
            Debug.Log($"{cardName} cura {flatHealing} HP a {target.playerName}");
        }
    }
}