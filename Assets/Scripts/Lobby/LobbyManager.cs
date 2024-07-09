using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    private const int maxPlayers = 5;

    public TMP_InputField roomNameInput;

    [SerializeField]
    private GameObject roomListContent;
    [SerializeField]
    private GameObject roomListItemPrefab;

    private List<RoomInfo> roomList = new List<RoomInfo>();

    private Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>();

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
           
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server!");

        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo roomInfo in roomList)
        {
            if (roomInfo.RemovedFromList || !roomInfo.IsOpen || !roomInfo.IsVisible)
            {
                if (!rooms.ContainsKey(roomInfo.Name))
                    return;

                Destroy(rooms[roomInfo.Name]);
                rooms.Remove(roomInfo.Name);
            }
            else if(!rooms.ContainsKey(roomInfo.Name))
            {
                GameObject roomListItem = Instantiate(roomListItemPrefab, roomListContent.transform);
                roomListItem.GetComponentInChildren<TextMeshProUGUI>().text = roomInfo.Name;
                roomListItem.GetComponentInChildren<Button>().onClick.AddListener(() => JoinRoom(roomInfo.Name));
                rooms.Add(roomInfo.Name, roomListItem);
            }
        }
    }

    public void CreateRoom()
    {
        if (!string.IsNullOrEmpty(roomNameInput.text))
        {
            SceneManager.LoadScene("Battleground");
            PhotonNetwork.JoinOrCreateRoom(roomNameInput.text, new RoomOptions { MaxPlayers = maxPlayers }, TypedLobby.Default);
        }
    }

    public void JoinRoom(string roomName)
    {
        SceneManager.LoadScene("Battleground");
        PhotonNetwork.JoinRoom(roomName);
    }

    //public void JoinRandomRoom()
    //{
    //    PhotonNetwork.JoinRandomRoom();
    //}

    //public override void OnJoinRandomFailed(short returnCode, string message)
    //{
    //    int gameMode = PlayerPrefs.GetInt("GameMode");
    //    if (gameMode == 1)
    //    {
    //        CreateMatchmakingRoom();
    //    }
    //    else
    //    {
    //        Debug.Log("Something went wrong. Create your own room");
    //        //PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayers });
    //    }
    //}

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
}