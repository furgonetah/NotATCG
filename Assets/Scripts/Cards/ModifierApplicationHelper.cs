using System.Collections.Generic;
using UnityEngine;

public static class ModifierApplicationHelper
{
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
            if (mod.type == ModifierType.MultiplyAllValues || mod.type == primaryType)
            {
                int previousValue = modifiedValue;
                modifiedValue = Mathf.RoundToInt(modifiedValue * mod.multiplier);
            }
            else if (mod.type == flatBonusType)
            {
                modifiedValue += mod.flatBonus;
            }
        }

        return modifiedValue;
    }
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
            }
        }

        return modifiedValue;
    }
    public static int GetFinalValue(int modifiedValue, int baseValue)
    {
        return modifiedValue > 0 ? modifiedValue : baseValue;
    }
}
