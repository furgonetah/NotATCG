using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;

/// <summary>
/// Maneja la representación visual e interacción de una carta en la UI.
/// Separado de Card.cs (lógica de juego) para mantener separación de responsabilidades.
/// </summary>
public class CardVisual : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, 
                           IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("Visual Settings")]
    public float selectionOffset = 50f; // Cuánto sube la carta al seleccionarla
    public float hoverScale = 1.1f; // Escala al hacer hover
    public float animationDuration = 0.2f;
    
    [Header("Drag Rotation Settings")]
    [Range(0f, 50f)]
    public float maxRotationAngle = 40f; // Ángulo máximo de rotación durante drag
    [Range(0.1f, 5f)]
    public float rotationSensitivity = 1f; // Sensibilidad de la rotación (ajustar al gusto: más alto = más dramático)
    public float rotationSmoothSpeed = 10f; // Velocidad de suavizado (más alto = más suave)
    
    [Header("State")]
    public bool selected = false;
    public Card cardData; // Referencia a la carta lógica (AttackCard, DefenseCard, etc.)
    
    [Header("References")]
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector3 originalScale;
    private int visualIndex; // Índice visual en la mano (para animaciones)
    
    // Variables para rotación durante drag
    private Vector2 lastDragPosition;
    private float currentRotation = 0f;
    private float targetRotation = 0f;
    
    // Events para comunicación con HandDisplayUI
    [HideInInspector] public UnityEvent<CardVisual> PointerEnterEvent = new UnityEvent<CardVisual>();
    [HideInInspector] public UnityEvent<CardVisual> PointerExitEvent = new UnityEvent<CardVisual>();
    [HideInInspector] public UnityEvent<CardVisual> BeginDragEvent = new UnityEvent<CardVisual>();
    [HideInInspector] public UnityEvent<CardVisual> EndDragEvent = new UnityEvent<CardVisual>();
    [HideInInspector] public UnityEvent<CardVisual> ClickEvent = new UnityEvent<CardVisual>();
    
    private Vector3 dragOffset;
    private bool isDragging = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Suavizar rotación durante el drag
        if (isDragging)
        {
            // Decaer suavemente la rotación objetivo hacia 0 cuando no hay movimiento
            targetRotation = Mathf.Lerp(targetRotation, 0f, Time.deltaTime * 5f);
            
            // Aplicar rotación actual con suavizado
            currentRotation = Mathf.Lerp(currentRotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);
            rectTransform.localRotation = Quaternion.Euler(0, 0, currentRotation);
        }
    }

    #region Event Handlers
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isDragging && !selected)
        {
            transform.DOScale(originalScale * hoverScale, animationDuration).SetEase(Ease.OutQuad);
        }
        PointerEnterEvent?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isDragging && !selected)
        {
            transform.DOScale(originalScale, animationDuration).SetEase(Ease.OutQuad);
        }
        PointerExitEvent?.Invoke(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Solo procesar clicks, no drags
        if (eventData.dragging)
            return;
            
        ClickEvent?.Invoke(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        
        // Calcular offset del mouse respecto al centro de la carta
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );
        
        dragOffset = rectTransform.localPosition - new Vector3(localPoint.x, localPoint.y, 0);
        
        // Inicializar tracking de rotación
        lastDragPosition = eventData.position;
        currentRotation = 0f;
        targetRotation = 0f;
        
        // Hacer la carta semi-transparente mientras se arrastra
        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false;
        
        // Escalar a tamaño normal si estaba con hover
        transform.DOScale(originalScale, animationDuration);
        
        BeginDragEvent?.Invoke(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        // Mover la carta siguiendo el mouse
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );
        
        rectTransform.localPosition = new Vector3(localPoint.x, localPoint.y, 0) + dragOffset;
        
        // Calcular rotación basada en la velocidad horizontal
        Vector2 dragDelta = eventData.position - lastDragPosition;
        float horizontalVelocity = dragDelta.x;
        
        // Calcular rotación objetivo (positivo = derecha/horario, negativo = izquierda/antihorario)
        targetRotation = Mathf.Clamp(-horizontalVelocity * rotationSensitivity, -maxRotationAngle, maxRotationAngle);
        
        // Guardar posición actual para próximo frame
        lastDragPosition = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        
        // Restaurar transparencia
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        
        // Resetear rotación suavemente a 0
        targetRotation = 0f;
        transform.DORotate(Vector3.zero, animationDuration).SetEase(Ease.OutQuad);
        
        EndDragEvent?.Invoke(this);
    }
    
    #endregion

    #region Selection Methods
    
    /// <summary>
    /// Selecciona la carta (la levanta visualmente)
    /// </summary>
    public void Select()
    {
        selected = true;
        transform.DOLocalMove(new Vector3(0, selectionOffset, 0), animationDuration).SetEase(Ease.OutBack);
        transform.DOScale(originalScale, animationDuration); // Volver a escala normal
    }

    /// <summary>
    /// Deselecciona la carta (la baja a su posición original)
    /// </summary>
    public void Deselect()
    {
        selected = false;
        transform.DOLocalMove(Vector3.zero, animationDuration).SetEase(Ease.OutQuad);
    }
    
    /// <summary>
    /// Actualiza la descripción de la carta en el UI
    /// </summary>
    public void UpdateDescription(string newDescription)
    {
        TMPro.TextMeshProUGUI cardText = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (cardText != null && cardData != null)
        {
            cardText.text = $"{cardData.cardName}\n{newDescription}";
        }
    }
    
    #endregion

    #region Visual Organization Methods
    
    /// <summary>
    /// Obtiene el índice del parent (slot) en la jerarquía
    /// </summary>
    public int ParentIndex()
    {
        return transform.parent.GetSiblingIndex();
    }

    /// <summary>
    /// Actualiza el índice visual de la carta (usado para animaciones de swap)
    /// </summary>
    public void UpdateIndex(int totalCards)
    {
        visualIndex = ParentIndex();
    }

    /// <summary>
    /// Animación cuando dos cartas intercambian posiciones
    /// </summary>
    public void Swap(int direction)
    {
        // Pequeña animación de "bounce" al intercambiar
        Vector3 offset = new Vector3(direction * 10f, 0, 0);
        transform.DOLocalMove(transform.localPosition + offset, 0.1f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => 
            {
                transform.DOLocalMove(selected ? new Vector3(0, selectionOffset, 0) : Vector3.zero, 0.1f)
                    .SetEase(Ease.InQuad);
            });
    }
    
    #endregion

    /// <summary>
    /// Retorna la carta a su posición correcta (seleccionada o no)
    /// </summary>
    public void ReturnToPosition(bool useTween = true)
    {
        Vector3 targetPos = selected ? new Vector3(0, selectionOffset, 0) : Vector3.zero;
        
        if (useTween)
        {
            transform.DOLocalMove(targetPos, animationDuration).SetEase(Ease.OutBack);
        }
        else
        {
            transform.localPosition = targetPos;
        }
    }
}