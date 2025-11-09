using UnityEngine;

/// <summary>
/// Sistema de modificadores que afectan temporalmente a las cartas.
/// Ejemplo: "Duplicar valores de la siguiente carta"
/// </summary>
[System.Serializable]
public class CardModifier
{
    public string modifierName;
    public ModifierType type;
    public float multiplier = 1f; // Para multiplicadores (2x = duplicar)
    public int flatBonus = 0; // Para bonos fijos (+5 daño)
    public int duration = 1; // Cuántas cartas afecta (1 = solo la siguiente)
    public Player owner; // Quién aplicó el modificador
    
    public CardModifier(string name, ModifierType modType, float mult = 1f, int flat = 0, int dur = 1)
    {
        modifierName = name;
        type = modType;
        multiplier = mult;
        flatBonus = flat;
        duration = dur;
    }
}

/// <summary>
/// Tipos de modificadores disponibles
/// </summary>
public enum ModifierType
{
    MultiplyAllValues,    // Multiplica todos los valores numéricos
    MultiplyDamage,       // Solo multiplica daño
    MultiplyHealing,      // Solo multiplica curación
    AddFlatDamage,        // Añade daño fijo
    AddFlatHealing,       // Añade curación fija
    MultiplyCardDraw      // Multiplica cartas a robar
}