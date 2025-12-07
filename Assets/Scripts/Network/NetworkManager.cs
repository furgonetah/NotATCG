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
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        isConnected = true;
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        isConnected = false;
        isInRoom = false;
    }

    public override void OnJoinedRoom()
    {
        isInRoom = true;
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("GameOnline");
            }
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("GameOnline");
            }
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
    }

    #endregion

    #region Public Methods

    public void CreateRoom(string roomName)
    {
        if (!isConnected)
        {
            return;
        }

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
            return;
        }

        PhotonNetwork.JoinRoom(roomName);
    }

    public void LeaveRoom()
    {
        if (isInRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }
    #endregion
}
