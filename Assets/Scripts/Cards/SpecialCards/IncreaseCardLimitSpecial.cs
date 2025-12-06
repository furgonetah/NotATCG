using UnityEngine;
using System.Collections.Generic;

public class IncreaseCardLimitSpecial : SpecialCard
{
    [Header("Card Limit Effect")]
    public int additionalCards = 1;
    
    protected override void ExecuteSpecialEffect(Player caster, Player target)
    {
        GameManager.Instance.turnManager.ModifyCardsPerTurn(additionalCards);
        Debug.Log($"{cardName}: Límite de cartas aumentado en {additionalCards}");
    }
    
    protected override Dictionary<string, string> GetCardValues()
    {
        Dictionary<string, string> values = new Dictionary<string, string>();
        values["cards"] = additionalCards.ToString();
        return values;
    }
}