using UnityEngine;
using DG.Tweening;
using TMPro;

public class BackGroundController : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Cámara cuyo color de fondo se va a cambiar (MainCamera)")]
    public Camera targetCamera;

    [Tooltip("Elemento TextMeshPro que se va a rotar (TitleText)")]
    public Transform titleText;

    [Header("Configuración de Colores")]
    [Tooltip("Colores entre los que se va a alternar")]
    public Color[] backgroundColors = new Color[]
    {
        new Color(0.2f, 0.3f, 0.5f), 
        new Color(0.3f, 0.2f, 0.4f), 
        new Color(0.2f, 0.4f, 0.4f), 
        new Color(0.4f, 0.3f, 0.3f)  
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
    private Vector3 initialRotation; 

    void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;

            if (targetCamera == null)
            {
                return;
            }
        }

        if (backgroundColors == null || backgroundColors.Length == 0)
        {
            return;
        }

        targetCamera.backgroundColor = backgroundColors[0];

        StartColorCycle();

        if (titleText != null)
        {
            StartTitleRotation();
        }
    }

    void StartColorCycle()
    {
        colorSequence = DOTween.Sequence();

        for (int i = 0; i < backgroundColors.Length; i++)
        {
            int nextIndex = (i + 1) % backgroundColors.Length;
            Color targetColor = backgroundColors[nextIndex];

            colorSequence.Append(
                targetCamera.DOColor(targetColor, transitionDuration)
                    .SetEase(Ease.InOutSine)
            );

            if (pauseBetweenTransitions > 0)
            {
                colorSequence.AppendInterval(pauseBetweenTransitions);
            }
        }

        colorSequence.SetLoops(-1);
    }

    void StartTitleRotation()
    {
        initialRotation = titleText.eulerAngles;

        rotationSequence = DOTween.Sequence();

        rotationSequence.Append(
            titleText.DORotate(new Vector3(initialRotation.x, initialRotation.y, initialRotation.z + maxRotationAngle), rotationDuration / 2f)
                .SetEase(Ease.InOutSine)
        );

        rotationSequence.Append(
            titleText.DORotate(new Vector3(initialRotation.x, initialRotation.y, initialRotation.z - maxRotationAngle), rotationDuration)
                .SetEase(Ease.InOutSine)
        );

        rotationSequence.Append(
            titleText.DORotate(new Vector3(initialRotation.x, initialRotation.y, initialRotation.z), rotationDuration / 2f)
                .SetEase(Ease.InOutSine)
        );

        rotationSequence.SetLoops(-1);
    }

    void OnDestroy()
    {
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
