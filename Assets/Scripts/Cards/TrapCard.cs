using UnityEngine;

public abstract class TrapCard : Card
{
    [Header("Trap Properties")]
    public bool isActive = false;
    public Player trapOwner;
    
    public override void Play(Player caster, Player target)
    {
        // Las trampas no se ejecutan inmediatamente, se colocan en el campo
        SetTrap(caster);
    }
    
    void SetTrap(Player owner)
    {
        trapOwner = owner;
        isActive = true;
        
        // Añadir a la lista de trampas activas
        GameManager.Instance.gameState.activeTraps.Add(this);
        
        Debug.Log($"{cardName} colocada por {owner.playerName}");
    }
    
    // Cada trampa define su propia condición de activación
    public override bool CheckTrapCondition(GameState state)
    {
        // Override en cada trampa específica
        return false;
    }
    
    // Método que se ejecuta cuando la trampa se activa
    public override void OnTrapActivate(Player caster, Player target)
    {
        ExecuteTrapEffect(caster, target);
        DestroyTrap();
    }
    
    // Efecto específico de cada trampa
    protected abstract void ExecuteTrapEffect(Player caster, Player target);
    
    // Destruir trampa después de usarse
    void DestroyTrap()
    {
        isActive = false;
        GameManager.Instance.gameState.activeTraps.Remove(this);
        Debug.Log($"{cardName} destruida");
    }
}

// EJEMPLO de trampa concreta:
public class RedirectHealTrap : TrapCard
{
    public override bool CheckTrapCondition(GameState state)
    {
        // Se activa cuando el oponente juega una carta de defensa
        return state.lastCardPlayed != null && 
               state.lastCardPlayed.cardType == CardType.Defense;
    }
    
    public override Player ModifyTarget(Player originalCaster, Player originalTarget)
    {
        // Cambia el objetivo de la curación al dueño de la trampa
        Debug.Log($"{cardName} activada! Redirigiendo curación a {trapOwner.playerName}");
        return trapOwner;
    }
    
    protected override void ExecuteTrapEffect(Player caster, Player target)
    {
        // El efecto ya se aplicó con ModifyTarget
        // Aquí podrías añadir efectos adicionales si la trampa hace más cosas
    }
}