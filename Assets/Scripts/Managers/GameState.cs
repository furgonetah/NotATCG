using UnityEngine;

public class GameState
{
    public GamePhase currentPhase;
    public Player activePlayer;
    public Player opponentPlayer;
    public int currentRound = 1;
    public int maxRounds = 3;

    // Info de turno actual (para cartas trampa)
    public Card lastCardPlayed;
    public int damageDealtThisTurn;
    public int healingDoneThisTurn;
    public bool playerDrewCardThisTurn;
    public int cardsPlayedThisTurn;

    // TODO: Todo lo referente al tiempo, especificado en GameManager
    public float timeRemaining;
    public bool isTimerActive = false;

    public bool playerDiedThisTurn = false;
    public Player deadPlayer = null;

    public GameState()
    {
        currentPhase = GamePhase.Menu;
    }

    public void ResetTurnData()
    {
        lastCardPlayed = null;
        damageDealtThisTurn = 0;
        healingDoneThisTurn = 0;
        playerDrewCardThisTurn = false;
        cardsPlayedThisTurn = 0;
        playerDiedThisTurn = false;
        deadPlayer = null;
    }

    public void SetPhase(GamePhase newPhase)
    {
        currentPhase = newPhase;
        Debug.Log($"GameState cambió a: {newPhase}");
    }
    public void SwapActivePlayer()
    {
        Player temp = activePlayer;
        activePlayer = opponentPlayer;
        opponentPlayer = temp;
    }
}

public enum GamePhase
{
    Menu,
    Setup,
    Playing,
    RoundEnd,
    GameEnd
}