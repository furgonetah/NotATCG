using UnityEngine;

public class AttackCard : Card
{
    [Header("Attack Properties")]
    public bool isPercentageDamage = false;
    
    [Header("Flat Damage")]
    public int flatDamage = 0;
    
    [Header("Percentage Damage")]
    [Range(0f, 1f)]
    public float percentageDamage = 0f; 
    
    public override void Play(Player caster, Player target)
    {
        if (isPercentageDamage)
        {
            target.TakeDamagePercentage(percentageDamage);
            Debug.Log($"{cardName} inflige {percentageDamage * 100}% de daño a {target.playerName}");
        }
        else
        {
            target.TakeDamage(flatDamage);
            Debug.Log($"{cardName} inflige {flatDamage} de daño a {target.playerName}");
        }
    }
}