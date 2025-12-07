using UnityEngine;

// Sistema de modificadores que afectan temporalmente a las cartas.
// Ejemplo: "Duplicar valores de la siguiente carta"
[System.Serializable]
public class CardModifier
{
    public string modifierName;
    public ModifierType type;
    public float multiplier = 1f; 
    public int flatBonus = 0; 
    public int duration = 1; 
    public Player owner;
    
    public CardModifier(string name, ModifierType modType, float mult = 1f, int flat = 0, int dur = 1)
    {
        modifierName = name;
        type = modType;
        multiplier = mult;
        flatBonus = flat;
        duration = dur;
    }
}

public enum ModifierType
{
    MultiplyAllValues,    
    MultiplyDamage,      
    MultiplyHealing,      
    AddFlatDamage,       
    AddFlatHealing,      
    MultiplyCardDraw      
}