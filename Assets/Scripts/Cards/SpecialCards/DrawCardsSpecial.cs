using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class DrawCardsSpecial : SpecialCard
{
    [Header("Draw Effect")]
    public int cardsToDraw = 2;
    
    private int modifiedCardsToDraw;
    
    protected override void ExecuteSpecialEffect(Player caster, Player target)
    {
        int finalCards = ModifierApplicationHelper.GetFinalValue(modifiedCardsToDraw, cardsToDraw);

        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            PhotonPlayer photonPlayer = caster.GetComponent<PhotonPlayer>();
            if (photonPlayer != null)
            {
                photonPlayer.DrawCards(finalCards);
            }
            else
            {
                caster.DrawCards(finalCards);
            }
        }
        else
        {
            caster.DrawCards(finalCards);
        }

        modifiedCardsToDraw = 0;
    }
    
    protected override Dictionary<string, string> GetCardValues()
    {
        Dictionary<string, string> values = new Dictionary<string, string>();
        values["cards"] = cardsToDraw.ToString();
        return values;
    }
    
    protected override void ApplyModifiersToSelf(List<CardModifier> modifiers)
    {
        modifiedCardsToDraw = ModifierApplicationHelper.ApplyMultiplierModifiers(
            cardsToDraw,
            modifiers,
            ModifierType.MultiplyCardDraw,
            cardName,
            "cartas"
        );
    }
}