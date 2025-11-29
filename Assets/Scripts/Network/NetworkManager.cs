using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance;

    [Header("Connection Status")]
    public bool isConnected = false;
    public bool isInRoom = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        Debug.Log("[NetworkManager] Conectando a Photon Cloud...");
        PhotonNetwork.ConnectUsingSettings();
    }

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        isConnected = true;
        Debug.Log("[NetworkManager] Conectado a Photon Master Server");

        // Unirse al lobby para poder crear/unirse a salas
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("[NetworkManager] Unido al lobby");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        isConnected = false;
        isInRoom = false;
        Debug.LogWarning($"[NetworkManager] Desconectado de Photon. Causa: {cause}");
    }

    public override void OnJoinedRoom()
    {
        isInRoom = true;
        Debug.Log($"[NetworkManager] Unido a sala: {PhotonNetwork.CurrentRoom.Name}");
        Debug.Log($"[NetworkManager] Jugadores en sala: {PhotonNetwork.CurrentRoom.PlayerCount}/2");

        // Si hay 2 jugadores, iniciar el juego
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            Debug.Log("[NetworkManager] Sala completa. Iniciando juego...");
            if (PhotonNetwork.IsMasterClient)
            {
                // El Master Client carga la escena de juego para todos
                PhotonNetwork.LoadLevel("SampleScene");
            }
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log($"[NetworkManager] Jugador {newPlayer.NickName} se unió a la sala");

        // Si ahora hay 2 jugadores, iniciar el juego
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            Debug.Log("[NetworkManager] Sala completa. Iniciando juego...");
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("SampleScene");
            }
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log($"[NetworkManager] Jugador {otherPlayer.NickName} dejó la sala");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"[NetworkManager] Error al crear sala: {message}");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"[NetworkManager] Error al unirse a sala: {message}");
    }

    #endregion

    #region Public Methods

    public void CreateRoom(string roomName)
    {
        if (!isConnected)
        {
            Debug.LogWarning("[NetworkManager] No conectado a Photon");
            return;
        }

        Debug.Log($"[NetworkManager] Creando sala: {roomName}");

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 2;
        options.IsVisible = true;
        options.IsOpen = true;

        PhotonNetwork.CreateRoom(roomName, options);
    }

    public void JoinRoom(string roomName)
    {
        if (!isConnected)
        {
            Debug.LogWarning("[NetworkManager] No conectado a Photon");
            return;
        }

        Debug.Log($"[NetworkManager] Uniéndose a sala: {roomName}");
        PhotonNetwork.JoinRoom(roomName);
    }

    public void LeaveRoom()
    {
        if (isInRoom)
        {
            Debug.Log("[NetworkManager] Saliendo de la sala...");
            PhotonNetwork.LeaveRoom();
        }
    }

    #endregion
}
