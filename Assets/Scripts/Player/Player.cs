using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    [Header("Player Info")]
    public string playerName;
    public bool isAI = false; //para tutorial si da tiempo

    [Header("Health")]
    public int maxHP = GameConstants.PLAYER_MAX_HP;
    public int currentHP = GameConstants.PLAYER_MAX_HP;

    [Header("Components")]
    public Hand hand;
    public Deck deck;

    [Header("Stats")]
    public int roundsWon = 0;
    
    [Header("Card Modifiers")]
    public List<CardModifier> activeModifiers = new List<CardModifier>();
    
    private GameState gameState;


    void Start()
    {
        currentHP = maxHP;
        if (GameManager.Current != null)
        {
            gameState = GameManager.Current.gameState;
        }
    }
    
    public void TakeDamage(int amount)
    {
        currentHP -= amount;

        if (currentHP <= 0)
        {
            currentHP = 0;
            OnDeath();
        }

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

    }

    public void HealPercentage(float percentage)
    {
        int healAmount = Mathf.CeilToInt(maxHP * percentage);
        Heal(healAmount);
    }

    public void DrawCard()
    {

        if (deck.IsEmpty())
        {
            return;
        }

        if (hand.IsEmpty() && gameState != null && gameState.cardsPlayedThisTurn > 0)
        {
            if (currentHP >= GameConstants.PLAYER_PENALTY_THRESHOLD)
            {
                TakeDamage(GameConstants.PLAYER_DRAW_PENALTY);
                Card drawnCard = deck.DrawCard();
                if (drawnCard != null)
                {
                    hand.AddCard(drawnCard);
                }
            }
            else
            {
            }
        }
        else
        {
            Card drawnCard = deck.DrawCard();
            if (drawnCard != null)
            {
                hand.AddCard(drawnCard);
            }
            else
            {
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
        if (GameManager.Current != null)
        {
            GameState gs = GameManager.Current.gameState;
            gs.playerDiedThisTurn = true;
            gs.deadPlayer = this;
        }
    }

    public void ResetForNewRound()
    {
        currentHP = maxHP;
        activeModifiers.Clear(); 
    }
}