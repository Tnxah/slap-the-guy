using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    private const int maxPlayers = 5;

    public TMP_InputField roomNameInput;

    [SerializeField]
    private GameObject roomsMenu;
    [SerializeField]
    private GameObject roomListContent;
    [SerializeField]
    private GameObject roomListItemPrefab;

    private List<RoomInfo> roomList = new List<RoomInfo>();

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();

        int gameMode = PlayerPrefs.GetInt("GameMode");
        if (gameMode == 1)
        {
            StartMatchmaking();
        }
        else
        {
            StartRoomSeeking();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server!");

        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform child in roomListContent.transform)
        {
            Destroy(child.gameObject);
        }

        this.roomList = roomList;
        foreach (RoomInfo roomInfo in roomList)
        {
            GameObject roomListItem = Instantiate(roomListItemPrefab, roomListContent.transform);
            roomListItem.GetComponentInChildren<TextMeshProUGUI>().text = roomInfo.Name;
            roomListItem.GetComponentInChildren<Button>().onClick.AddListener(() => JoinRoom(roomInfo.Name));
        }
    }

    public void CreateRoom()
    {
        if (!string.IsNullOrEmpty(roomNameInput.text))
        {
            PhotonNetwork.CreateRoom(roomNameInput.text, new RoomOptions { MaxPlayers = maxPlayers }, TypedLobby.Default);
        }
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        int gameMode = PlayerPrefs.GetInt("GameMode");
        if (gameMode == 1)
        {
            CreateMatchmakingRoom();
        }
        else
        {
            Debug.Log("Something went wrong. Create your own room");
            //PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayers });
        }
    }

    public void StartMatchmaking()
    {
        int playerRating = FindObjectOfType<PlayerRatingManager>().PlayerRating;
        PhotonNetwork.JoinRandomRoom(null, maxPlayers, MatchmakingMode.FillRoom, null, playerRating.ToString());
    }

    void CreateMatchmakingRoom()
    {
        int playerRating = FindObjectOfType<PlayerRatingManager>().PlayerRating;
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = maxPlayers };
        PhotonNetwork.CreateRoom("Matchmaking_" + playerRating, roomOptions);
    }

    public void StartRoomSeeking()
    {
        roomsMenu.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room.");
        roomsMenu.SetActive(false);
    }
}