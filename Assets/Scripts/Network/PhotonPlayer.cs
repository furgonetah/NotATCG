using UnityEngine;
using Photon.Pun;

/// <summary>
/// Wrapper para Player con sincronización de red.
/// NOTA: En la implementación actual, como ambos clientes ejecutan el mismo ExecuteQueueRPC,
/// las llamadas a TakeDamage/Heal ya están sincronizadas automáticamente.
/// Este componente está disponible para sincronización explícita si es necesaria en el futuro.
/// </summary>
public class PhotonPlayer : MonoBehaviourPunCallbacks
{
    public Player player;

    void Awake()
    {
        // photonView ya está disponible automáticamente desde MonoBehaviourPunCallbacks

        // Obtener Player del mismo GameObject
        if (player == null)
        {
            player = GetComponent<Player>();
        }
    }

    /// <summary>
    /// Método para sincronizar daño explícitamente vía RPC (opcional).
    /// Usar solo si necesitas aplicar daño fuera del flujo normal de cartas.
    /// </summary>
    public void TakeDamageNetwork(int amount)
    {
        photonView.RPC("TakeDamageRPC", RpcTarget.All, amount);
    }

    [PunRPC]
    void TakeDamageRPC(int amount)
    {
        player.TakeDamage(amount);
        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] {player.playerName} recibe {amount} de daño (RPC)");
    }

    /// <summary>
    /// Método para sincronizar curación explícitamente vía RPC (opcional).
    /// Usar solo si necesitas aplicar curación fuera del flujo normal de cartas.
    /// </summary>
    public void HealNetwork(int amount)
    {
        photonView.RPC("HealRPC", RpcTarget.All, amount);
    }

    [PunRPC]
    void HealRPC(int amount)
    {
        player.Heal(amount);
        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] {player.playerName} se cura {amount} PV (RPC)");
    }

    /// <summary>
    /// Método para sincronizar aplicación de modificadores vía RPC (opcional).
    /// </summary>
    public void ApplyModifierNetwork(string modifierName, int multiplierType, float multiplier, int flatBonus, int duration)
    {
        photonView.RPC("ApplyModifierRPC", RpcTarget.All, modifierName, multiplierType, multiplier, flatBonus, duration);
    }

    [PunRPC]
    void ApplyModifierRPC(string modifierName, int multiplierType, float multiplier, int flatBonus, int duration)
    {
        CardModifier modifier = new CardModifier(modifierName, (ModifierType)multiplierType, multiplier, flatBonus, duration);
        modifier.owner = player;
        player.activeModifiers.Add(modifier);
        Debug.Log($"[{(PhotonNetwork.IsMasterClient ? "MASTER" : "CLIENT")}] Modificador '{modifierName}' aplicado a {player.playerName} (RPC)");
    }
}
