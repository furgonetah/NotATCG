using UnityEngine;
using DG.Tweening;
using TMPro;

/// <summary>
/// Controlador de fondo que cambia gradualmente el color de la cámara
/// entre varios valores predefinidos de forma continua
/// También rota suavemente el TitleText
/// </summary>
public class BackGroundController : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Cámara cuyo color de fondo se va a cambiar (normalmente MainCamera)")]
    public Camera targetCamera;

    [Tooltip("Elemento TextMeshPro que se va a rotar (TitleText)")]
    public Transform titleText;

    [Header("Configuración de Colores")]
    [Tooltip("Colores entre los que se va a alternar")]
    public Color[] backgroundColors = new Color[]
    {
        new Color(0.2f, 0.3f, 0.5f), // Azul oscuro
        new Color(0.3f, 0.2f, 0.4f), // Púrpura oscuro
        new Color(0.2f, 0.4f, 0.4f), // Verde azulado
        new Color(0.4f, 0.3f, 0.3f)  // Marrón rojizo
    };


    [Header("Configuración de Animación de Color")]
    [Tooltip("Duración de la transición entre colores (en segundos)")]
    [Range(1f, 10f)]
    public float transitionDuration = 3f;

    [Tooltip("Pausa entre transiciones (en segundos)")]
    [Range(0f, 5f)]
    public float pauseBetweenTransitions = 0.5f;

    [Header("Configuración de Rotación del Título")]
    [Tooltip("Ángulo máximo de rotación (se rotará entre -maxRotationAngle y +maxRotationAngle)")]
    [Range(5f, 45f)]
    public float maxRotationAngle = 15f;

    [Tooltip("Duración de cada ciclo de rotación completo (en segundos)")]
    [Range(1f, 10f)]
    public float rotationDuration = 2f;

    private int currentColorIndex = 0;
    private Sequence colorSequence;
    private Sequence rotationSequence;
    private Vector3 initialRotation; // Rotación inicial del TitleText

    void Start()
    {
        // Si no se asignó una cámara, intentar obtener la MainCamera
        if (targetCamera == null)
        {
            targetCamera = Camera.main;

            if (targetCamera == null)
            {
                Debug.LogError("[BackGroundController] No se encontró una cámara. Asigna la MainCamera en el Inspector.");
                return;
            }
        }

        // Validar que hay colores configurados
        if (backgroundColors == null || backgroundColors.Length == 0)
        {
            Debug.LogError("[BackGroundController] No hay colores configurados en el array.");
            return;
        }

        // Establecer el color inicial
        targetCamera.backgroundColor = backgroundColors[0];

        // Iniciar el ciclo de cambio de colores
        StartColorCycle();

        // Iniciar la rotación del título si está asignado
        if (titleText != null)
        {
            StartTitleRotation();
        }
        else
        {
            Debug.LogWarning("[BackGroundController] TitleText no asignado. No se aplicará rotación.");
        }
    }

    void StartColorCycle()
    {
        // Crear una secuencia que se repetirá infinitamente
        colorSequence = DOTween.Sequence();

        // Agregar transiciones para cada color
        for (int i = 0; i < backgroundColors.Length; i++)
        {
            int nextIndex = (i + 1) % backgroundColors.Length;
            Color targetColor = backgroundColors[nextIndex];

            // Agregar la transición de color
            colorSequence.Append(
                targetCamera.DOColor(targetColor, transitionDuration)
                    .SetEase(Ease.InOutSine)
            );

            // Agregar pausa si es mayor que 0
            if (pauseBetweenTransitions > 0)
            {
                colorSequence.AppendInterval(pauseBetweenTransitions);
            }
        }

        // Hacer que la secuencia se repita infinitamente
        colorSequence.SetLoops(-1);
    }

    void StartTitleRotation()
    {
        // Guardar la rotación inicial configurada en la escena de Unity
        initialRotation = titleText.eulerAngles;

        // Crear una secuencia que se repetirá infinitamente
        rotationSequence = DOTween.Sequence();

        // Rotar de la posición inicial a (inicial + maxRotationAngle)
        rotationSequence.Append(
            titleText.DORotate(new Vector3(initialRotation.x, initialRotation.y, initialRotation.z + maxRotationAngle), rotationDuration / 2f)
                .SetEase(Ease.InOutSine)
        );

        // Rotar de (inicial + maxRotationAngle) a (inicial - maxRotationAngle)
        rotationSequence.Append(
            titleText.DORotate(new Vector3(initialRotation.x, initialRotation.y, initialRotation.z - maxRotationAngle), rotationDuration)
                .SetEase(Ease.InOutSine)
        );

        // Rotar de (inicial - maxRotationAngle) de vuelta a la rotación inicial
        rotationSequence.Append(
            titleText.DORotate(new Vector3(initialRotation.x, initialRotation.y, initialRotation.z), rotationDuration / 2f)
                .SetEase(Ease.InOutSine)
        );

        // Hacer que la secuencia se repita infinitamente
        rotationSequence.SetLoops(-1);
    }

    void OnDestroy()
    {
        // Limpiar las secuencias al destruir el objeto
        if (colorSequence != null)
        {
            colorSequence.Kill();
        }

        if (rotationSequence != null)
        {
            rotationSequence.Kill();
        }
    }
}
