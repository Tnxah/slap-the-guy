using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomManager : MonoBehaviourPunCallbacks
{
    private GameplayController gameplayController;

    private Dictionary<Player, Transform> playerSpawnPoints = new Dictionary<Player, Transform>();

    [SerializeField]
    private List<Transform> spawnPoints;

    private void Awake()
    {
        gameplayController = GetComponent<GameplayController>();
        GameplayController.onGameStart += CloseRoom;
    }

    private void CreatePlayer()
    {
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player entered: " + newPlayer.NickName);
        OnPlayerEnteredOrLeft();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Player left: " + otherPlayer.NickName);
        OnPlayerEnteredOrLeft();
    }

    [PunRPC]
    private void UpdatePlayerListAndSpawnPoints()
    {
        if (gameplayController.isStarted) return;

        List<Player> sortedPlayers = PhotonNetwork.PlayerList.OrderBy(p => p.ActorNumber).ToList();

        playerSpawnPoints.Clear();

        for (int i = 0; i < sortedPlayers.Count; i++)
        {
            if (i < spawnPoints.Count)
            {
                playerSpawnPoints[sortedPlayers[i]] = spawnPoints[i];
                MovePlayerToSpawnPoint(sortedPlayers[i], spawnPoints[i]);
            }
        }
    }

    private void MovePlayerToSpawnPoint(Player player, Transform spawnPoint)
    {
        PhotonView[] photonViews = FindObjectsOfType<PhotonView>();
        foreach (PhotonView view in photonViews)
        {
            if (view.Owner == player)
            {
                view.transform.position = spawnPoint.position;
                
                break;
            }
        }
    }

    private void CheckRoomAvailability()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom && !gameplayController.isStarted)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                CloseRoom();
            }
            else
            {
                OpenRoom();
            }
        }
    }

    private void OnPlayerEnteredOrLeft()
    {
        photonView.RPC("UpdatePlayerListAndSpawnPoints", RpcTarget.All);
        CheckRoomAvailability();
    }

    private void CloseRoom()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
        {
            Debug.Log("Room Closed");
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
    }
    private  void OpenRoom()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
        {
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        CreatePlayer();
        OnPlayerEnteredOrLeft();
    }
}
