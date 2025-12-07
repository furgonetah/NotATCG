using UnityEngine;
using System.Collections.Generic;

public abstract class SpecialCard : Card
{
    public override void Play(Player caster, Player target)
    {
        ApplyModifiersToCard(caster); 
        ExecuteSpecialEffect(caster, target);
    }
    protected abstract void ExecuteSpecialEffect(Player caster, Player target);
}
