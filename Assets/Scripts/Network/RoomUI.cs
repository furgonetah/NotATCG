using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class RoomUI : MonoBehaviour
{
    [Header("Main Menu Panel")]
    public GameObject mainMenuPanel;
    public TMP_InputField roomNameInput;
    public Button createRoomButton;
    public TMP_InputField joinCodeInput;
    public Button joinRoomButton;
    public TextMeshProUGUI statusText;

    [Header("Waiting Room Panel")]
    public GameObject waitingRoomPanel;
    public TextMeshProUGUI roomNameText;
    public TextMeshProUGUI playersCountText;

    void Start()
    {
        // Setup inicial
        mainMenuPanel.SetActive(true);
        waitingRoomPanel.SetActive(false);

        // Conectar botones
        createRoomButton.onClick.AddListener(OnCreateRoomClicked);
        joinRoomButton.onClick.AddListener(OnJoinRoomClicked);

        // Estado inicial
        UpdateStatusText("Conectando a Photon...");
        SetButtonsInteractable(false);

        // Esperar a que NetworkManager se conecte
        InvokeRepeating("CheckConnection", 0.5f, 0.5f);
    }

    void CheckConnection()
    {
        if (NetworkManager.Instance != null && NetworkManager.Instance.isConnected)
        {
            UpdateStatusText("Conectado. Crea o únete a una sala.");
            SetButtonsInteractable(true);
            CancelInvoke("CheckConnection");
        }
    }

    void Update()
    {
        // Actualizar UI de sala de espera si estamos en una sala
        if (NetworkManager.Instance != null && NetworkManager.Instance.isInRoom)
        {
            if (!waitingRoomPanel.activeSelf)
            {
                mainMenuPanel.SetActive(false);
                waitingRoomPanel.SetActive(true);
            }

            UpdateWaitingRoom();
        }
    }

    void OnCreateRoomClicked()
    {
        string roomName = roomNameInput.text.Trim();

        if (string.IsNullOrEmpty(roomName))
        {
            UpdateStatusText("Error: El nombre de la sala no puede estar vacío");
            return;
        }

        UpdateStatusText($"Creando sala '{roomName}'...");
        SetButtonsInteractable(false);

        NetworkManager.Instance.CreateRoom(roomName);
    }

    void OnJoinRoomClicked()
    {
        string roomCode = joinCodeInput.text.Trim();

        if (string.IsNullOrEmpty(roomCode))
        {
            UpdateStatusText("Error: El código de sala no puede estar vacío");
            return;
        }

        UpdateStatusText($"Uniéndose a sala '{roomCode}'...");
        SetButtonsInteractable(false);

        NetworkManager.Instance.JoinRoom(roomCode);
    }

    void UpdateStatusText(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
        Debug.Log($"[RoomUI] {message}");
    }

    void SetButtonsInteractable(bool interactable)
    {
        createRoomButton.interactable = interactable;
        joinRoomButton.interactable = interactable;
    }

    void UpdateWaitingRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            roomNameText.text = $"Sala: {PhotonNetwork.CurrentRoom.Name}";
            playersCountText.text = $"Jugadores: {PhotonNetwork.CurrentRoom.PlayerCount}/2";

            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                playersCountText.text += "\n¡Iniciando partida...!";
            }
            else
            {
                playersCountText.text += "\nEsperando al segundo jugador...";
            }
        }
    }
}
