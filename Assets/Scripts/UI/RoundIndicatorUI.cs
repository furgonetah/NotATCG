using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// Gestiona los indicadores visuales de rondas ganadas (3 círculos que cambian de sprite)
/// Similar al sistema de iconos de tipo de carta
/// </summary>
public class RoundIndicatorUI : MonoBehaviour
{
    [Header("Round Circles")]
    [SerializeField] private List<Image> roundCircles = new List<Image>(3);

    [Header("Sprites (asignar en Unity)")]
    [SerializeField] private Sprite emptySprite; // Círculo vacío
    [SerializeField] private Sprite player1WinSprite; // Sprite cuando gana J1
    [SerializeField] private Sprite player2WinSprite; // Sprite cuando gana J2

    [Header("Animation")]
    [SerializeField] private float scalePopDuration = 0.3f;
    [SerializeField] private float scalePopAmount = 1.3f;

    [Header("Players")]
    [SerializeField] private Player player1;
    [SerializeField] private Player player2;

    private int lastPlayer1Wins = 0;
    private int lastPlayer2Wins = 0;
    private List<int> winnerHistory = new List<int>(); // 1 = player1, 2 = player2

    void Start()
    {
        // Inicializar todos los círculos como vacíos
        ResetIndicators();

        // Obtener referencias a los jugadores si no están asignadas
        if ((player1 == null || player2 == null) && GameManager.Current != null)
        {
            player1 = GameManager.Current.player1;
            player2 = GameManager.Current.player2;
        }

        Debug.Log($"[RoundIndicatorUI] Initialized with {roundCircles.Count} circles");
    }

    void Update()
    {
        // Detectar cambios en las rondas ganadas
        if (player1 != null && player2 != null)
        {
            // Detectar si Player 1 ganó una ronda
            if (player1.roundsWon > lastPlayer1Wins)
            {
                winnerHistory.Add(1);
                lastPlayer1Wins = player1.roundsWon;
                UpdateIndicators();
                Debug.Log($"[RoundIndicatorUI] Player 1 ganó ronda {winnerHistory.Count}");
            }

            // Detectar si Player 2 ganó una ronda
            if (player2.roundsWon > lastPlayer2Wins)
            {
                winnerHistory.Add(2);
                lastPlayer2Wins = player2.roundsWon;
                UpdateIndicators();
                Debug.Log($"[RoundIndicatorUI] Player 2 ganó ronda {winnerHistory.Count}");
            }
        }
    }

    /// <summary>
    /// Actualiza los indicadores basándose en el historial de ganadores
    /// </summary>
    private void UpdateIndicators()
    {
        Debug.Log($"[RoundIndicatorUI] UpdateIndicators - History: {string.Join(",", winnerHistory)}");

        for (int i = 0; i < roundCircles.Count; i++)
        {
            if (roundCircles[i] == null)
            {
                Debug.LogWarning($"[RoundIndicatorUI] Circle {i} is null!");
                continue;
            }

            if (i < winnerHistory.Count)
            {
                // Usar el historial real de ganadores para determinar el sprite
                Sprite winnerSprite = (winnerHistory[i] == 1) ? player1WinSprite : player2WinSprite;
                SetCircleSprite(i, winnerSprite, true);
            }
            else
            {
                // Círculo vacío
                SetCircleSprite(i, emptySprite, false);
            }
        }
    }

    /// <summary>
    /// Cambia el sprite de un círculo con animación opcional
    /// </summary>
    private void SetCircleSprite(int index, Sprite sprite, bool animate)
    {
        if (index >= roundCircles.Count || roundCircles[index] == null)
        {
            Debug.LogWarning($"[RoundIndicatorUI] Cannot set sprite for circle {index} - invalid or null");
            return;
        }

        Image circle = roundCircles[index];

        // Cambiar el sprite
        circle.sprite = sprite;

        Debug.Log($"[RoundIndicatorUI] SetCircleSprite {index} - Sprite: {sprite?.name ?? "null"}, Animate: {animate}");

        // Animación de "pop" al cambiar
        if (animate && sprite != emptySprite)
        {
            circle.transform.DOKill(); // Matar animaciones previas
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

    /// <summary>
    /// Reinicia todos los indicadores a estado vacío
    /// </summary>
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
        Debug.Log("[RoundIndicatorUI] Indicators reset");
    }

    /// <summary>
    /// Configura manualmente los círculos (útil para setup en Unity Editor)
    /// </summary>
    public void SetupCircles(Image circle1, Image circle2, Image circle3)
    {
        roundCircles.Clear();
        roundCircles.Add(circle1);
        roundCircles.Add(circle2);
        roundCircles.Add(circle3);

        ResetIndicators();
    }

    /// <summary>
    /// Configura los sprites que se usarán (útil para cambiarlos en runtime)
    /// </summary>
    public void SetSprites(Sprite empty, Sprite player1Win, Sprite player2Win)
    {
        emptySprite = empty;
        player1WinSprite = player1Win;
        player2WinSprite = player2Win;

        // Actualizar sprites actuales
        UpdateIndicators();
    }

#if UNITY_EDITOR
    // Método helper para configurar desde el inspector
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

        Debug.Log($"Encontrados {roundCircles.Count} círculos de ronda");
    }

    [ContextMenu("Test - Player 1 Wins Round")]
    private void TestPlayer1Win()
    {
        if (Application.isPlaying && player1 != null)
        {
            player1.roundsWon++;
            Debug.Log("Test: Player 1 wins!");
        }
    }

    [ContextMenu("Test - Player 2 Wins Round")]
    private void TestPlayer2Win()
    {
        if (Application.isPlaying && player2 != null)
        {
            player2.roundsWon++;
            Debug.Log("Test: Player 2 wins!");
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
            Debug.Log("Test: Reset!");
        }
    }
#endif
}
