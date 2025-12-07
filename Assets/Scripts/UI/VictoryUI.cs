using UnityEngine;
using TMPro;

public class VictoryUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject victoryPanel;
    public TextMeshProUGUI victoryText;
    
    void Start()
    {
        victoryPanel.SetActive(false); 
    }
    
    public void ShowVictory(Player winner, int winnerScore, int loserScore)
    {
        victoryPanel.SetActive(true);
        victoryText.text = $"¡{winner.playerName} gana la partida!";
    }
}