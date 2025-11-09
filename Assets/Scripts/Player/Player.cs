using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    [Header("Player Info")]
    public string playerName;
    public bool isAI = false; //para tutorial si da tiempo

    [Header("Health")]
    public int maxHP = 100;
    public int currentHP = 100;

    [Header("Components")]
    public Hand hand;
    public Deck deck;

    [Header("Stats")]
    public int roundsWon = 0;
    
    [Header("Card Modifiers")]
    public List<CardModifier> activeModifiers = new List<CardModifier>(); // Modificadores activos
    
    private GameState gameState;


    void Start()
    {
        currentHP = maxHP;
        gameState = GameManager.Instance.gameState;
    }
    
    public void TakeDamage(int amount)
    {
        currentHP -= amount;

        if (currentHP <= 0)
        {
            currentHP = 0;
            OnDeath();
        }

        // TODO: UI y sonido de daño
        Debug.Log($"{playerName} recibe {amount} de daño. HP: {currentHP}");
    }

    public void TakeDamagePercentage(float percentage)
    {
        int damage = Mathf.FloorToInt(currentHP * percentage);
        TakeDamage(damage);
    }

    public void Heal(int amount)
    {
        currentHP += amount;

        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }

        Debug.Log($"{playerName} se cura {amount} HP. HP: {currentHP}");
    }

    //TODO: revisar bien la cura porcentual cuando se aplique, que ahora mismo no me funciona el cerebro
    public void HealPercentage(float percentage)
    {
        int healAmount = Mathf.CeilToInt(maxHP * percentage);
        Heal(healAmount);
    }

    public void DrawCard()
    {
        Debug.Log($"{playerName} intenta robar carta. Deck vacío: {deck.IsEmpty()}, Mano vacía: {hand.IsEmpty()}");

        if (deck.IsEmpty())
        {
            Debug.Log($"{playerName} no tiene cartas en el mazo. Solo puede usar Ataque Básico.");
            return;
        }

        if (hand.IsEmpty() && gameState != null && gameState.cardsPlayedThisTurn > 0)
        {
            if (currentHP >= 11)
            {
                TakeDamage(10);
                Card drawnCard = deck.DrawCard();
                if (drawnCard != null)
                {
                    hand.AddCard(drawnCard);
                    Debug.Log($"{playerName} robó carta con penalización: {drawnCard.cardName}");
                }
            }
            else
            {
                Debug.Log($"{playerName} no tiene suficientes PV para robar cartas.");
            }
        }
        else
        {
            Card drawnCard = deck.DrawCard();
            if (drawnCard != null)
            {
                hand.AddCard(drawnCard);
                Debug.Log($"{playerName} robó carta: {drawnCard.cardName}");
            }
            else
            {
                Debug.Log($"{playerName} intentó robar pero DrawCard devolvió null");
            }
        }
    }
    
    public void DrawCards(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            DrawCard();
        }
    }

    private void OnDeath()
    {
        Debug.Log($"{playerName} ha sido derrotado!");
        GameState gameState = GameManager.Instance.gameState;
        gameState.playerDiedThisTurn = true;
        gameState.deadPlayer = this;
    }

    public void ResetForNewRound()
    {
        currentHP = maxHP;
        activeModifiers.Clear(); // Limpiar modificadores al empezar nueva ronda
    }
}