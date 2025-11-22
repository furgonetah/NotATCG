using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

/// <summary>
/// Maneja la visualización de la mano del jugador con sistema de reordenamiento por drag & drop,
/// selección visual de cartas, y integración con CardQueue.
/// </summary>
public class HandDisplayUI : MonoBehaviour
{
    [Header("Prefab References")]
    [SerializeField] private GameObject cardSlotPrefab; // Prefab del CardSlot
    [SerializeField] private GameObject cardVisualPrefab; // Prefab del CardVisual (con imagen, texto, etc.)
    
    [Header("Container")]
    [SerializeField] private Transform cardContainer; // El transform donde se crean los slots
    
    [Header("Player Reference")]
    [SerializeField] private Player player;
    
    [Header("Card Queue")]
    [SerializeField] private CardQueue cardQueue;
    
    [Header("Visual Settings")]
    [SerializeField] private bool tweenCardReturn = true; // Animar retorno de cartas al soltar
    [SerializeField] private Color selectedColor = new Color(0.8f, 1f, 0.8f, 1f); // Tinte verde claro para cartas seleccionadas
    
    // State
    private List<CardVisual> cardVisuals = new List<CardVisual>();
    private CardVisual selectedCardForDrag; // Carta siendo arrastrada actualmente
    private CardVisual hoveredCard; // Carta bajo el cursor
    private bool isCrossing = false; // Previene múltiples swaps simultáneos
    private int lastHandCount = -1;
    
    // Tracking de cartas seleccionadas (añadidas a la queue)
    private List<CardVisual> selectedCards = new List<CardVisual>();
    
    private RectTransform rect;

    void Start()
    {
        rect = cardContainer.GetComponent<RectTransform>();
        
        // Inicializar la mano
        RefreshHand();
    }

    void Update()
    {
        // Refrescar si el número de cartas cambió
        if (player.hand.cardsInHand.Count != lastHandCount)
        {
            RefreshHand();
            lastHandCount = player.hand.cardsInHand.Count;
        }
        
        // Manejar reordenamiento durante drag
        HandleCardReordering();
    }

    /// <summary>
    /// Refresca completamente la visualización de la mano
    /// </summary>
    void RefreshHand()
    {
        // Limpiar slots y cartas existentes
        ClearHand();
        
        // Si no hay cartas, no hacer nada
        if (player.hand.cardsInHand.Count == 0)
        {
            return;
        }
        
        // Crear un slot por cada carta en la mano
        for (int i = 0; i < player.hand.cardsInHand.Count; i++)
        {
            // Crear slot
            GameObject slotObj = Instantiate(cardSlotPrefab, cardContainer);
            CardSlot slot = slotObj.GetComponent<CardSlot>();
            
            // Crear visual de carta
            GameObject cardObj = Instantiate(cardVisualPrefab, slotObj.transform);
            CardVisual cardVisual = cardObj.GetComponent<CardVisual>();
            
            // Configurar datos de la carta
            Card cardData = player.hand.cardsInHand[i];
            cardVisual.cardData = cardData;

            // Actualizar icono de tipo
            cardVisual.UpdateTypeIcon();

            // Configurar los 3 componentes de texto por separado
            TextMeshProUGUI titleText = cardObj.transform.Find("TitleText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI descriptionText = cardObj.transform.Find("DescriptionText")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI numberText = cardObj.transform.Find("NumberText")?.GetComponent<TextMeshProUGUI>();

            // Configurar título
            if (titleText != null)
            {
                titleText.text = cardData.cardName;
            }

            // Configurar descripción dinámica con placeholders reemplazados
            if (descriptionText != null)
            {
                string description = cardData.GetDynamicDescription(null); // null = sin modificadores aún
                descriptionText.text = description;
            }

            // Configurar valor numérico
            if (numberText != null)
            {
                string numericValue = ExtractNumericValue(cardData);
                numberText.text = numericValue;
            }
            
            // Configurar imagen de fondo si existe
            Image cardImage = cardObj.GetComponent<Image>();
            if (cardImage != null && cardData.cardArt != null)
            {
                cardImage.sprite = cardData.cardArt;
            }
            
            // Suscribir eventos
            cardVisual.PointerEnterEvent.AddListener(OnCardPointerEnter);
            cardVisual.PointerExitEvent.AddListener(OnCardPointerExit);
            cardVisual.BeginDragEvent.AddListener(OnCardBeginDrag);
            cardVisual.EndDragEvent.AddListener(OnCardEndDrag);
            cardVisual.ClickEvent.AddListener(OnCardClicked);
            
            // Añadir a lista
            cardVisuals.Add(cardVisual);
            slot.SetCard(cardVisual);
        }
        
        // Forzar actualización del layout
        Canvas.ForceUpdateCanvases();
        
        // Esperar un frame para actualizar índices visuales
        StartCoroutine(UpdateVisualIndices());
    }

    /// <summary>
    /// Limpia todos los slots y cartas de la mano
    /// </summary>
    void ClearHand()
    {
        // Destruir todos los slots (y sus cartas hijas)
        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }
        
        cardVisuals.Clear();
        selectedCards.Clear();
    }

    IEnumerator UpdateVisualIndices()
    {
        yield return new WaitForSeconds(0.1f);
        
        for (int i = 0; i < cardVisuals.Count; i++)
        {
            if (cardVisuals[i] != null)
            {
                cardVisuals[i].UpdateIndex(cardContainer.childCount);
            }
        }
    }

    #region Event Handlers

    void OnCardPointerEnter(CardVisual card)
    {
        hoveredCard = card;
    }

    void OnCardPointerExit(CardVisual card)
    {
        if (hoveredCard == card)
        {
            hoveredCard = null;
        }
    }

    void OnCardBeginDrag(CardVisual card)
    {
        selectedCardForDrag = card;
    }

    void OnCardEndDrag(CardVisual card)
    {
        if (selectedCardForDrag == null)
            return;

        // Retornar carta a su posición (seleccionada o no)
        selectedCardForDrag.ReturnToPosition(tweenCardReturn);

        // Forzar recalculo de layout
        rect.sizeDelta += Vector2.right;
        rect.sizeDelta -= Vector2.right;

        selectedCardForDrag = null;
    }

    /// <summary>
    /// Maneja el click en una carta para seleccionarla/deseleccionarla
    /// </summary>
    void OnCardClicked(CardVisual cardVisual)
    {
        // Si ya está seleccionada, deseleccionar
        if (selectedCards.Contains(cardVisual))
        {
            DeselectCard(cardVisual);
        }
        // Si no está seleccionada, intentar seleccionar
        else
        {
            SelectCard(cardVisual);
        }
    }

    #endregion

    #region Selection Logic

    /// <summary>
    /// Selecciona una carta (añadiéndola a la queue)
    /// </summary>
    void SelectCard(CardVisual cardVisual)
    {
        // Verificar si se puede añadir más cartas a la queue
        if (!cardQueue.CanAddCard())
        {
            Debug.Log("No puedes jugar más cartas este turno.");
            return;
        }

        // Añadir a la queue
        cardQueue.AddCardToQueue(cardVisual.cardData);
        
        // Añadir a lista de seleccionadas
        selectedCards.Add(cardVisual);
        
        // Aplicar visual de selección
        cardVisual.Select();
        ApplySelectionTint(cardVisual, true);
        
        // Actualizar descripción con modificadores activos
        UpdateCardDescriptionWithModifiers(cardVisual);
        
        Debug.Log($"Carta seleccionada: {cardVisual.cardData.cardName}");
    }

    /// <summary>
    /// Deselecciona una carta (quitándola de la queue)
    /// </summary>
    void DeselectCard(CardVisual cardVisual)
    {
        // Quitar de la queue
        cardQueue.RemoveCardFromQueue(cardVisual.cardData);
        
        // Quitar de lista de seleccionadas
        selectedCards.Remove(cardVisual);
        
        // Quitar visual de selección
        cardVisual.Deselect();
        ApplySelectionTint(cardVisual, false);
        
        // Restaurar descripción base (sin modificadores)
        UpdateCardDescriptionBase(cardVisual);
        
        Debug.Log($"Carta deseleccionada: {cardVisual.cardData.cardName}");
    }
    
    /// <summary>
    /// Actualiza la descripción de la carta mostrando valores con modificadores activos
    /// </summary>
    void UpdateCardDescriptionWithModifiers(CardVisual cardVisual)
    {
        if (cardVisual.cardData == null || player == null)
            return;
            
        // Obtener descripción con modificadores activos del jugador
        string modifiedDescription = cardVisual.cardData.GetDynamicDescription(player.activeModifiers);
        cardVisual.UpdateDescription(modifiedDescription);
        
        if (player.activeModifiers.Count > 0)
        {
            Debug.Log($"Descripción actualizada con modificadores: {modifiedDescription}");
        }
    }
    
    /// <summary>
    /// Restaura la descripción base de la carta (sin modificadores)
    /// </summary>
    void UpdateCardDescriptionBase(CardVisual cardVisual)
    {
        if (cardVisual.cardData == null)
            return;
            
        // Obtener descripción base sin modificadores
        string baseDescription = cardVisual.cardData.GetDynamicDescription(null);
        cardVisual.UpdateDescription(baseDescription);
    }

    /// <summary>
    /// Aplica un tinte de color a la carta seleccionada
    /// </summary>
    void ApplySelectionTint(CardVisual cardVisual, bool selected)
    {
        Image cardImage = cardVisual.GetComponent<Image>();
        if (cardImage != null)
        {
            cardImage.color = selected ? selectedColor : Color.white;
        }
    }

    /// <summary>
    /// Limpia todas las selecciones (llamado al finalizar turno)
    /// </summary>
    public void ClearSelection()
    {
        Debug.Log($"ClearSelection llamado para {player.playerName}. Cartas seleccionadas: {selectedCards.Count}");
        
        // Crear copia de la lista para evitar modificación durante iteración
        List<CardVisual> cardsToDeselect = new List<CardVisual>(selectedCards);
        
        foreach (CardVisual card in cardsToDeselect)
        {
            if (card != null)
            {
                card.Deselect();
                ApplySelectionTint(card, false);
            }
        }
        
        selectedCards.Clear();
        
        // Limpiar también la referencia de drag si existe
        selectedCardForDrag = null;
        hoveredCard = null;
    }

    #endregion

    #region Drag Reordering Logic

    /// <summary>
    /// Maneja el reordenamiento de cartas durante el drag
    /// </summary>
    void HandleCardReordering()
    {
        if (selectedCardForDrag == null)
            return;

        if (isCrossing)
            return;

        // Verificar si la carta arrastrada cruzó con otra
        for (int i = 0; i < cardVisuals.Count; i++)
        {
            if (cardVisuals[i] == selectedCardForDrag)
                continue;

            // Si la carta arrastrada está a la derecha de otra carta
            if (selectedCardForDrag.transform.position.x > cardVisuals[i].transform.position.x)
            {
                // Y su índice es menor (está a la izquierda en jerarquía)
                if (selectedCardForDrag.ParentIndex() < cardVisuals[i].ParentIndex())
                {
                    SwapCards(i);
                    break;
                }
            }

            // Si la carta arrastrada está a la izquierda de otra carta
            if (selectedCardForDrag.transform.position.x < cardVisuals[i].transform.position.x)
            {
                // Y su índice es mayor (está a la derecha en jerarquía)
                if (selectedCardForDrag.ParentIndex() > cardVisuals[i].ParentIndex())
                {
                    SwapCards(i);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Intercambia las posiciones de dos cartas
    /// </summary>
    void SwapCards(int targetIndex)
    {
        isCrossing = true;

        Transform draggedParent = selectedCardForDrag.transform.parent;
        Transform targetParent = cardVisuals[targetIndex].transform.parent;

        // Intercambiar parents (orden visual)
        cardVisuals[targetIndex].transform.SetParent(draggedParent);
        cardVisuals[targetIndex].transform.localPosition = cardVisuals[targetIndex].selected 
            ? new Vector3(0, cardVisuals[targetIndex].selectionOffset, 0) 
            : Vector3.zero;
            
        selectedCardForDrag.transform.SetParent(targetParent);

        // Actualizar slots
        CardSlot draggedSlot = draggedParent.GetComponent<CardSlot>();
        CardSlot targetSlot = targetParent.GetComponent<CardSlot>();
        
        draggedSlot.SetCard(cardVisuals[targetIndex]);
        targetSlot.SetCard(selectedCardForDrag);

        // NUEVO: Sincronizar con player.hand.cardsInHand
        SyncHandOrderWithVisuals();

        isCrossing = false;

        // Animación de swap
        if (cardVisuals[targetIndex] != null)
        {
            bool swapIsRight = cardVisuals[targetIndex].ParentIndex() > selectedCardForDrag.ParentIndex();
            cardVisuals[targetIndex].Swap(swapIsRight ? -1 : 1);
        }

        // Actualizar índices visuales
        foreach (CardVisual card in cardVisuals)
        {
            if (card != null)
            {
                card.UpdateIndex(cardContainer.childCount);
            }
        }
    }
    
    /// <summary>
    /// Sincroniza el orden de player.hand.cardsInHand con el orden visual de las cartas
    /// </summary>
    void SyncHandOrderWithVisuals()
    {
        if (player == null || player.hand == null)
            return;
        
        // Crear nueva lista ordenada según la jerarquía visual
        List<Card> newOrder = new List<Card>();
        
        // Iterar por los slots en orden (de izquierda a derecha)
        for (int i = 0; i < cardContainer.childCount; i++)
        {
            Transform slotTransform = cardContainer.GetChild(i);
            CardSlot slot = slotTransform.GetComponent<CardSlot>();
            
            if (slot != null && slot.cardInSlot != null && slot.cardInSlot.cardData != null)
            {
                newOrder.Add(slot.cardInSlot.cardData);
            }
        }
        
        // Reemplazar la lista de cartas en la mano del jugador
        player.hand.cardsInHand.Clear();
        player.hand.cardsInHand.AddRange(newOrder);
        
        Debug.Log($"Orden de mano sincronizado: {newOrder.Count} cartas");
    }

    #endregion

    #region Public Control Methods

    /// <summary>
    /// Muestra la mano (activa el GameObject)
    /// </summary>
    public void ShowHand()
    {
        gameObject.SetActive(true);
        RefreshHand();
    }

    /// <summary>
    /// Oculta la mano (desactiva el GameObject)
    /// </summary>
    public void HideHand()
    {
        ClearSelection();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Fuerza un refresh completo de la mano
    /// </summary>
    public void ForceRefresh()
    {
        lastHandCount = -1; // Forzar refresh en próximo Update
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Extrae el valor numérico de una carta para mostrarlo en NumberText
    /// </summary>
    string ExtractNumericValue(Card card)
    {
        if (card is AttackCard attackCard)
        {
            return attackCard.isPercentageDamage
                ? $"{attackCard.percentageDamage * 100:F0}%"
                : attackCard.flatDamage.ToString();
        }
        else if (card is DefenseCard defenseCard)
        {
            return defenseCard.isPercentageHealing
                ? $"{defenseCard.percentageHealing * 100:F0}%"
                : defenseCard.flatHealing.ToString();
        }
        else if (card is DrawCardsSpecial drawCard)
        {
            return drawCard.cardsToDraw.ToString();
        }

        return "?";
    }

    #endregion
}