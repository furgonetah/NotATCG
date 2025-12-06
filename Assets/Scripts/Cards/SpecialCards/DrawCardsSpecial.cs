using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class DrawCardsSpecial : SpecialCard
{
    [Header("Draw Effect")]
    public int cardsToDraw = 2;
    
    // Valor modificado
    private int modifiedCardsToDraw;
    
    protected override void ExecuteSpecialEffect(Player caster, Player target)
    {
        int finalCards = ModifierApplicationHelper.GetFinalValue(modifiedCardsToDraw, cardsToDraw);

        // Si estamos en multijugador, usar PhotonPlayer para logs deterministas
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            PhotonPlayer photonPlayer = caster.GetComponent<PhotonPlayer>();
            if (photonPlayer != null)
            {
                // DrawCards de PhotonPlayer es determinista (se ejecuta igual en ambos clientes)
                photonPlayer.DrawCards(finalCards);
            }
            else
            {
                Debug.LogWarning($"{cardName}: PhotonPlayer no encontrado en {caster.playerName}, usando Player.DrawCards");
                caster.DrawCards(finalCards);
            }
        }
        else
        {
            // Modo local (sin red)
            caster.DrawCards(finalCards);
            Debug.Log($"{cardName}: {caster.playerName} roba {finalCards} cartas");
        }

        // Resetear
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