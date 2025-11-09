using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public abstract class Card : MonoBehaviour
{
    [Header("Card Info")]
    public string cardName;
    public CardType cardType;
    [TextArea(3, 5)]
    public string cardDescriptionTemplate; // Template con placeholders: "Inflige {damage} de daño"
    
    [Header("Visual")]
    public Sprite cardArt;
    
    // Descripción generada dinámicamente (no serializada)
    [HideInInspector]
    public string currentDescription;
    
    public abstract void Play(Player caster, Player target);

    public virtual bool CheckTrapCondition(GameState state)
    {
        return false;
    }
    
    public virtual Player ModifyTarget(Player originalCaster, Player originalTarget)
    {
        return originalTarget;
    }
    
    public virtual void OnTrapActivate(Player caster, Player target)
    {
        // Override en TrapCard
    }
    
    /// <summary>
    /// Genera la descripción de la carta reemplazando placeholders con valores reales.
    /// Placeholders disponibles: {damage}, {healing}, {cards}, {percentage}, etc.
    /// </summary>
    public virtual string GetDynamicDescription(List<CardModifier> activeModifiers = null)
    {
        // Obtener valores base de la carta
        Dictionary<string, string> values = GetCardValues();
        
        // Aplicar modificadores si existen
        if (activeModifiers != null && activeModifiers.Count > 0)
        {
            values = ApplyModifiersToValues(values, activeModifiers);
        }
        
        // Reemplazar placeholders en el template
        string description = cardDescriptionTemplate;
        
        foreach (var kvp in values)
        {
            string placeholder = "{" + kvp.Key + "}";
            description = description.Replace(placeholder, kvp.Value);
        }
        
        return description;
    }
    
    /// <summary>
    /// Obtiene los valores base de la carta como diccionario.
    /// Override en cada tipo de carta para añadir sus valores específicos.
    /// </summary>
    protected virtual Dictionary<string, string> GetCardValues()
    {
        // Diccionario base vacío, cada carta lo llena con sus valores
        return new Dictionary<string, string>();
    }
    
    /// <summary>
    /// Aplica modificadores activos a los valores de la carta.
    /// </summary>
    protected virtual Dictionary<string, string> ApplyModifiersToValues(Dictionary<string, string> baseValues, List<CardModifier> modifiers)
    {
        Dictionary<string, string> modifiedValues = new Dictionary<string, string>(baseValues);
        
        foreach (CardModifier mod in modifiers)
        {
            switch (mod.type)
            {
                case ModifierType.MultiplyAllValues:
                    modifiedValues = MultiplyAllNumericValues(modifiedValues, mod.multiplier);
                    break;
                    
                case ModifierType.MultiplyDamage:
                    if (modifiedValues.ContainsKey("damage"))
                    {
                        int value = int.Parse(modifiedValues["damage"]);
                        modifiedValues["damage"] = Mathf.RoundToInt(value * mod.multiplier).ToString();
                    }
                    break;
                    
                case ModifierType.MultiplyHealing:
                    if (modifiedValues.ContainsKey("healing"))
                    {
                        int value = int.Parse(modifiedValues["healing"]);
                        modifiedValues["healing"] = Mathf.RoundToInt(value * mod.multiplier).ToString();
                    }
                    break;
                    
                case ModifierType.AddFlatDamage:
                    if (modifiedValues.ContainsKey("damage"))
                    {
                        int value = int.Parse(modifiedValues["damage"]);
                        modifiedValues["damage"] = (value + mod.flatBonus).ToString();
                    }
                    break;
                    
                case ModifierType.AddFlatHealing:
                    if (modifiedValues.ContainsKey("healing"))
                    {
                        int value = int.Parse(modifiedValues["healing"]);
                        modifiedValues["healing"] = (value + mod.flatBonus).ToString();
                    }
                    break;
                    
                case ModifierType.MultiplyCardDraw:
                    if (modifiedValues.ContainsKey("cards"))
                    {
                        int value = int.Parse(modifiedValues["cards"]);
                        modifiedValues["cards"] = Mathf.RoundToInt(value * mod.multiplier).ToString();
                    }
                    break;
            }
        }
        
        return modifiedValues;
    }
    
    /// <summary>
    /// Multiplica todos los valores numéricos en el diccionario.
    /// </summary>
    private Dictionary<string, string> MultiplyAllNumericValues(Dictionary<string, string> values, float multiplier)
    {
        Dictionary<string, string> result = new Dictionary<string, string>();
        
        foreach (var kvp in values)
        {
            if (int.TryParse(kvp.Value, out int numValue))
            {
                result[kvp.Key] = Mathf.RoundToInt(numValue * multiplier).ToString();
            }
            else
            {
                result[kvp.Key] = kvp.Value; // No numérico, mantener igual
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// Aplica modificadores activos a los valores reales de la carta antes de ejecutarla.
    /// Llamar desde Play() de cada carta.
    /// </summary>
    protected void ApplyModifiersToCard(Player caster)
    {
        if (caster.activeModifiers.Count == 0)
            return;
            
        // Aplicar modificadores a esta carta
        ApplyModifiersToSelf(caster.activeModifiers);
        
        // Consumir modificadores (reducir duración)
        ConsumeModifiers(caster);
    }
    
    /// <summary>
    /// Aplica los modificadores a los valores internos de la carta.
    /// Override en cada tipo de carta.
    /// </summary>
    protected virtual void ApplyModifiersToSelf(List<CardModifier> modifiers)
    {
        // Override en AttackCard, DefenseCard, etc.
    }
    
    /// <summary>
    /// Consume modificadores (reduce duración y elimina si llegan a 0)
    /// </summary>
    private void ConsumeModifiers(Player caster)
    {
        for (int i = caster.activeModifiers.Count - 1; i >= 0; i--)
        {
            caster.activeModifiers[i].duration--;
            
            if (caster.activeModifiers[i].duration <= 0)
            {
                Debug.Log($"Modificador '{caster.activeModifiers[i].modifierName}' consumido");
                caster.activeModifiers.RemoveAt(i);
            }
        }
    }
}

public enum CardType
{
    Attack,
    Defense,
    Special,
    Trap
}