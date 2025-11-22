using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase helper que centraliza la lógica de aplicación de modificadores a cartas.
/// Elimina código duplicado en AttackCard, DefenseCard y SpecialCard.
/// </summary>
public static class ModifierApplicationHelper
{
    /// <summary>
    /// Aplica modificadores a un valor base según el tipo de modificador.
    /// </summary>
    /// <param name="baseValue">Valor base de la carta (damage, healing, cards, etc.)</param>
    /// <param name="modifiers">Lista de modificadores activos del jugador</param>
    /// <param name="primaryType">Tipo de modificador primario (MultiplyDamage, MultiplyHealing, etc.)</param>
    /// <param name="flatBonusType">Tipo de modificador de bonus plano (AddFlatDamage, AddFlatHealing)</param>
    /// <param name="cardName">Nombre de la carta (para debug logs)</param>
    /// <param name="valueName">Nombre del valor modificado (para debug logs: "daño", "curación", "cartas")</param>
    /// <returns>Valor modificado después de aplicar todos los modificadores</returns>
    public static int ApplyModifiers(
        int baseValue,
        List<CardModifier> modifiers,
        ModifierType primaryType,
        ModifierType flatBonusType,
        string cardName = "",
        string valueName = "valor")
    {
        if (modifiers == null || modifiers.Count == 0)
            return baseValue;

        int modifiedValue = baseValue;

        foreach (CardModifier mod in modifiers)
        {
            // Multiplicadores (específicos o generales)
            if (mod.type == ModifierType.MultiplyAllValues || mod.type == primaryType)
            {
                int previousValue = modifiedValue;
                modifiedValue = Mathf.RoundToInt(modifiedValue * mod.multiplier);

                Debug.Log($"[{cardName}] Modificador '{mod.modifierName}' aplicado: {valueName} {previousValue} → {modifiedValue} (x{mod.multiplier})");
            }
            // Bonus planos
            else if (mod.type == flatBonusType)
            {
                modifiedValue += mod.flatBonus;
                Debug.Log($"[{cardName}] Modificador '{mod.modifierName}' aplicado: {valueName} +{mod.flatBonus} = {modifiedValue}");
            }
        }

        return modifiedValue;
    }

    /// <summary>
    /// Versión simplificada para casos donde solo se usan multiplicadores.
    /// </summary>
    public static int ApplyMultiplierModifiers(
        int baseValue,
        List<CardModifier> modifiers,
        ModifierType primaryType,
        string cardName = "",
        string valueName = "valor")
    {
        if (modifiers == null || modifiers.Count == 0)
            return baseValue;

        int modifiedValue = baseValue;

        foreach (CardModifier mod in modifiers)
        {
            if (mod.type == ModifierType.MultiplyAllValues || mod.type == primaryType)
            {
                int previousValue = modifiedValue;
                modifiedValue = Mathf.RoundToInt(modifiedValue * mod.multiplier);

                Debug.Log($"[{cardName}] Modificador '{mod.modifierName}' aplicado: {valueName} {previousValue} → {modifiedValue} (x{mod.multiplier})");
            }
        }

        return modifiedValue;
    }

    /// <summary>
    /// Obtiene el valor final a usar en Play(): usa el modificado si existe, sino el base.
    /// </summary>
    /// <param name="modifiedValue">Valor modificado (puede ser 0 si no se modificó)</param>
    /// <param name="baseValue">Valor base original</param>
    /// <returns>El valor a usar en la ejecución de la carta</returns>
    public static int GetFinalValue(int modifiedValue, int baseValue)
    {
        return modifiedValue > 0 ? modifiedValue : baseValue;
    }
}
