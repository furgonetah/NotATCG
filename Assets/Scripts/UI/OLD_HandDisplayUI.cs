using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OLD_HandDisplayUI : MonoBehaviour
{
    [Header("References")]
    public GameObject cardUIPrefab;
    public Transform cardContainer;
    public CardQueue cardQueue;
    
    [Header("Player")]
    public Player player;
    
    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.green;
    
    private Dictionary<GameObject, Card> cardUIToCard = new Dictionary<GameObject, Card>();
    private List<GameObject> selectedCardUIs = new List<GameObject>();
    private int lastHandCount = -1;
    
    void Update()
    {
        // Solo refrescar si el número de cartas cambió
        if (player.hand.cardsInHand.Count != lastHandCount)
        {
            RefreshHand();
            lastHandCount = player.hand.cardsInHand.Count;
        }
    }
    
    void RefreshHand()
    {
        // Limpiar cartas UI existentes
        foreach (GameObject cardUI in cardUIToCard.Keys)
        {
            Destroy(cardUI);
        }
        cardUIToCard.Clear();
        selectedCardUIs.Clear();
        
        // Crear UI para cada carta en mano
        for (int i = 0; i < player.hand.cardsInHand.Count; i++)
        {
            Card card = player.hand.cardsInHand[i];
            GameObject cardUI = Instantiate(cardUIPrefab, cardContainer);
            
            // Configurar texto
            TextMeshProUGUI cardText = cardUI.GetComponentInChildren<TextMeshProUGUI>();
            cardText.text = $"{card.cardName}\n{card.cardDescription}";
            
            // Guardar relación UI-Card
            cardUIToCard[cardUI] = card;
            
            // Configurar botón
            Button button = cardUI.GetComponent<Button>();
            GameObject cardUIRef = cardUI; // Captura para el lambda
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnCardClicked(cardUIRef));
            
            // Color inicial
            Image buttonImage = cardUI.GetComponent<Image>();
            buttonImage.color = normalColor;
        }
    }
    
    void OnCardClicked(GameObject cardUI)
    {
        Card card = cardUIToCard[cardUI];
        Image buttonImage = cardUI.GetComponent<Image>();
        
        // Si ya está seleccionada, deseleccionar
        if (selectedCardUIs.Contains(cardUI))
        {
            selectedCardUIs.Remove(cardUI);
            cardQueue.RemoveCardFromQueue(card);
            buttonImage.color = normalColor;
            Debug.Log($"Carta deseleccionada: {card.cardName}");
        }
        // Si no está seleccionada, intentar seleccionar
        else
        {
            if (cardQueue.CanAddCard())
            {
                selectedCardUIs.Add(cardUI);
                cardQueue.AddCardToQueue(card);
                buttonImage.color = selectedColor;
                Debug.Log($"Carta seleccionada: {card.cardName}");
            }
            else
            {
                Debug.Log("No puedes jugar más cartas este turno.");
            }
        }
    }
    
    // Limpiar selección al finalizar turno
    public void ClearSelection()
    {
        selectedCardUIs.Clear();
    }
}