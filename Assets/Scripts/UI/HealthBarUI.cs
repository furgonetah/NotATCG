using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

/// <summary>
/// Gestiona la visualización de la barra de vida de un jugador con animaciones suaves
/// </summary>
public class HealthBarUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Visual Settings")]
    [SerializeField] private Gradient healthGradient;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private Ease animationEase = Ease.OutCubic;

    [Header("Color Presets (si no usas gradiente)")]
    [SerializeField] private Color highHealthColor = new Color(0.2f, 0.8f, 0.2f); // Verde
    [SerializeField] private Color mediumHealthColor = new Color(0.9f, 0.9f, 0.2f); // Amarillo
    [SerializeField] private Color lowHealthColor = new Color(0.9f, 0.2f, 0.2f); // Rojo

    [Header("Thresholds")]
    [SerializeField] private float lowHealthThreshold = 0.3f; // 30%
    [SerializeField] private float mediumHealthThreshold = 0.6f; // 60%

    private Player targetPlayer;
    private int lastKnownHP;
    private Tween healthTween;

    void Awake()
    {
        // Buscar el Slider si no está asignado
        if (healthSlider == null)
        {
            healthSlider = GetComponent<Slider>();
            if (healthSlider == null)
            {
                healthSlider = GetComponentInChildren<Slider>();
            }
        }

        // Buscar el Fill Image si no está asignado
        if (fillImage == null && healthSlider != null && healthSlider.fillRect != null)
        {
            fillImage = healthSlider.fillRect.GetComponent<Image>();
        }

        Debug.Log($"[HealthBarUI] Awake - Slider: {healthSlider != null}, FillImage: {fillImage != null}");
    }

    /// <summary>
    /// Inicializa la barra de vida para un jugador específico
    /// </summary>
    public void Initialize(Player player)
    {
        targetPlayer = player;

        if (healthSlider != null)
        {
            // Configurar el slider correctamente
            healthSlider.minValue = 0;
            healthSlider.maxValue = player.maxHP;
            healthSlider.value = player.currentHP;
            healthSlider.interactable = false;

            // Asegurar que la dirección es de izquierda a derecha
            healthSlider.direction = Slider.Direction.LeftToRight;

            // Ocultar el background del slider (la barra blanca)
            Image backgroundImage = healthSlider.GetComponentInChildren<Image>();
            if (backgroundImage != null && backgroundImage != fillImage)
            {
                // Buscar específicamente el Background
                Transform bgTransform = healthSlider.transform.Find("Background");
                if (bgTransform != null)
                {
                    Image bg = bgTransform.GetComponent<Image>();
                    if (bg != null)
                    {
                        bg.enabled = false; // Desactivar el fondo blanco
                    }
                }
            }
        }

        lastKnownHP = player.currentHP;
        UpdateHealthDisplay(player.currentHP, player.maxHP, false);

        Debug.Log($"[HealthBarUI] Initialized for {player.playerName} - HP: {player.currentHP}/{player.maxHP}");
    }

    void Update()
    {
        if (targetPlayer != null && targetPlayer.currentHP != lastKnownHP)
        {
            UpdateHealthDisplay(targetPlayer.currentHP, targetPlayer.maxHP, true);
            lastKnownHP = targetPlayer.currentHP;
        }
    }

    /// <summary>
    /// Actualiza la visualización de la barra de vida
    /// </summary>
    private void UpdateHealthDisplay(int currentHP, int maxHP, bool animate)
    {
        if (healthSlider == null) return;

        float targetValue = currentHP;
        float healthPercentage = (float)currentHP / maxHP;

        // Animar el cambio de valor
        if (animate && animationDuration > 0)
        {
            healthTween?.Kill();
            healthTween = DOTween.To(
                () => healthSlider.value,
                x => healthSlider.value = x,
                targetValue,
                animationDuration
            ).SetEase(animationEase);
        }
        else
        {
            healthSlider.value = targetValue;
        }

        // Actualizar color basado en salud
        UpdateHealthColor(healthPercentage);

        // Actualizar texto
        if (healthText != null)
        {
            healthText.text = $"{currentHP}/{maxHP}";
        }
    }

    /// <summary>
    /// Actualiza el color de la barra según el porcentaje de vida
    /// </summary>
    private void UpdateHealthColor(float healthPercentage)
    {
        if (fillImage == null)
        {
            Debug.LogWarning("[HealthBarUI] FillImage is null, cannot update color");
            return;
        }

        Color targetColor;

        // Usar colores por umbrales (más simple y confiable)
        if (healthPercentage <= lowHealthThreshold)
        {
            targetColor = lowHealthColor;
        }
        else if (healthPercentage <= mediumHealthThreshold)
        {
            targetColor = mediumHealthColor;
        }
        else
        {
            targetColor = highHealthColor;
        }

        fillImage.color = targetColor;
        Debug.Log($"[HealthBarUI] Color updated - HP%: {healthPercentage:P0}, Color: {targetColor}");
    }

    /// <summary>
    /// Fuerza una actualización inmediata sin animación
    /// </summary>
    public void ForceUpdate()
    {
        if (targetPlayer != null)
        {
            UpdateHealthDisplay(targetPlayer.currentHP, targetPlayer.maxHP, false);
            lastKnownHP = targetPlayer.currentHP;
        }
    }

    void OnDestroy()
    {
        healthTween?.Kill();
    }
}
