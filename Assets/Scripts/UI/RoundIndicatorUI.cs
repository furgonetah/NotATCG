using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;


public class RoundIndicatorUI : MonoBehaviour
{
    [Header("Round Circles")]
    [SerializeField] private List<Image> roundCircles = new List<Image>(3);

    [Header("Sprites (asignar en Unity)")]
    [SerializeField] private Sprite emptySprite; 
    [SerializeField] private Sprite player1WinSprite;
    [SerializeField] private Sprite player2WinSprite;

    [Header("Animation")]
    [SerializeField] private float scalePopDuration = 0.3f;
    [SerializeField] private float scalePopAmount = 1.3f;

    [Header("Players")]
    [SerializeField] private Player player1;
    [SerializeField] private Player player2;

    private int lastPlayer1Wins = 0;
    private int lastPlayer2Wins = 0;
    private List<int> winnerHistory = new List<int>(); 

    void Start()
    {
        ResetIndicators();

        if ((player1 == null || player2 == null) && GameManager.Current != null)
        {
            player1 = GameManager.Current.player1;
            player2 = GameManager.Current.player2;
        }

    }

    void Update()
    {
        if (player1 != null && player2 != null)
        {
            if (player1.roundsWon > lastPlayer1Wins)
            {
                winnerHistory.Add(1);
                lastPlayer1Wins = player1.roundsWon;
                UpdateIndicators();
            }

            if (player2.roundsWon > lastPlayer2Wins)
            {
                winnerHistory.Add(2);
                lastPlayer2Wins = player2.roundsWon;
                UpdateIndicators();
            }
        }
    }

    private void UpdateIndicators()
    {

        for (int i = 0; i < roundCircles.Count; i++)
        {
            if (roundCircles[i] == null)
            {
                continue;
            }

            if (i < winnerHistory.Count)
            {
                Sprite winnerSprite = (winnerHistory[i] == 1) ? player1WinSprite : player2WinSprite;
                SetCircleSprite(i, winnerSprite, true);
            }
            else
            {
                SetCircleSprite(i, emptySprite, false);
            }
        }
    }

    private void SetCircleSprite(int index, Sprite sprite, bool animate)
    {
        if (index >= roundCircles.Count || roundCircles[index] == null)
        {
            return;
        }

        Image circle = roundCircles[index];

        circle.sprite = sprite;

        if (animate && sprite != emptySprite)
        {
            circle.transform.DOKill(); 
            circle.transform.localScale = Vector3.one;

            circle.transform.DOScale(scalePopAmount, scalePopDuration * 0.5f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    circle.transform.DOScale(1f, scalePopDuration * 0.5f).SetEase(Ease.InBack);
                });
        }
        else
        {
            circle.transform.localScale = Vector3.one;
        }
    }

    public void ResetIndicators()
    {
        for (int i = 0; i < roundCircles.Count; i++)
        {
            if (roundCircles[i] != null)
            {
                roundCircles[i].sprite = emptySprite;
                roundCircles[i].transform.localScale = Vector3.one;
            }
        }

        lastPlayer1Wins = 0;
        lastPlayer2Wins = 0;
        winnerHistory.Clear();
    }

    public void SetupCircles(Image circle1, Image circle2, Image circle3)
    {
        roundCircles.Clear();
        roundCircles.Add(circle1);
        roundCircles.Add(circle2);
        roundCircles.Add(circle3);

        ResetIndicators();
    }

    public void SetSprites(Sprite empty, Sprite player1Win, Sprite player2Win)
    {
        emptySprite = empty;
        player1WinSprite = player1Win;
        player2WinSprite = player2Win;

        UpdateIndicators();
    }

#if UNITY_EDITOR
    [ContextMenu("Find All Round Circle Images")]
    private void FindRoundCircles()
    {
        roundCircles.Clear();
        Image[] images = GetComponentsInChildren<Image>();

        foreach (Image img in images)
        {
            if (img.gameObject.name.Contains("RoundCircle"))
            {
                roundCircles.Add(img);
            }
        }

    }

    [ContextMenu("Test - Player 1 Wins Round")]
    private void TestPlayer1Win()
    {
        if (Application.isPlaying && player1 != null)
        {
            player1.roundsWon++;
        }
    }

    [ContextMenu("Test - Player 2 Wins Round")]
    private void TestPlayer2Win()
    {
        if (Application.isPlaying && player2 != null)
        {
            player2.roundsWon++;
        }
    }

    [ContextMenu("Test - Reset")]
    private void TestReset()
    {
        if (Application.isPlaying)
        {
            if (player1 != null) player1.roundsWon = 0;
            if (player2 != null) player2.roundsWon = 0;
            ResetIndicators();
        }
    }
#endif
}
