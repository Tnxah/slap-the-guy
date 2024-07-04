using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomManager : MonoBehaviourPunCallbacks
{
    private Dictionary<Player, Transform> playerSpawnPoints = new Dictionary<Player, Transform>();

    [SerializeField]
    private List<Transform> spawnPoints;

    private void CreatePlayer()
    {
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player entered: " + newPlayer.NickName);
        photonView.RPC("UpdatePlayerListAndSpawnPoints", RpcTarget.All);
        CheckRoomAvailability();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Player left: " + otherPlayer.NickName);
        photonView.RPC("UpdatePlayerListAndSpawnPoints", RpcTarget.All);
        CheckRoomAvailability();
    }

    // Method to update the player list and assign spawn points
    [PunRPC]
    private void UpdatePlayerListAndSpawnPoints()
    {
        if (GameplayController.isStarted) return;
        List<Player> sortedPlayers = PhotonNetwork.PlayerList.OrderBy(p => p.ActorNumber).ToList();

        playerSpawnPoints.Clear();

        // Assign spawn points to players based on their order in the sorted list
        for (int i = 0; i < sortedPlayers.Count; i++)
        {
            if (i < spawnPoints.Count)
            {
                playerSpawnPoints[sortedPlayers[i]] = spawnPoints[i];
                // Move the player to the assigned spawn point
                MovePlayerToSpawnPoint(sortedPlayers[i], spawnPoints[i]);
            }
        }
    }

    // Method to move the player to the assigned spawn point
    private void MovePlayerToSpawnPoint(Player player, Transform spawnPoint)
    {
        // Find the PhotonView of the player
        PhotonView[] photonViews = FindObjectsOfType<PhotonView>();
        foreach (PhotonView view in photonViews)
        {
            if (view.Owner == player)
            {
                view.transform.position = spawnPoint.position;
                view.transform.rotation = spawnPoint.rotation;
                break;
            }
        }
    }

    private void CheckRoomAvailability()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom && !GameplayController.isStarted)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers) 
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;  // Close the room
                PhotonNetwork.CurrentRoom.IsVisible = false;  // Hide the room from the list
            }
            else
            {
                PhotonNetwork.CurrentRoom.IsOpen = true;
                PhotonNetwork.CurrentRoom.IsVisible = true;
            }
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        CreatePlayer();
        photonView.RPC("UpdatePlayerListAndSpawnPoints", RpcTarget.All);
        CheckRoomAvailability();
    }
}
