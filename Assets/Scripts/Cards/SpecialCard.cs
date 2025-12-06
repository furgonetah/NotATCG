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
