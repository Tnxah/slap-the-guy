using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    private const int maxPlayers = 5;

    public static event Action onJoinedRoomCallback;

    public static string roomName = "test";

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server!");
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { MaxPlayers = maxPlayers }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room.");
        onJoinedRoomCallback?.Invoke();
    }
}