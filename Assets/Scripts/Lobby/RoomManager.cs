using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public List<Transform> spawnpoints;

    private void Awake()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            CreatePlayer();
        }
    }

    private void CreatePlayer()
    {
        Vector3 spawnPos;
        int spawnIndex = PhotonNetwork.CurrentRoom.PlayerCount - 1;

        spawnPos = spawnpoints[spawnIndex].position;

        PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        CreatePlayer();
    }
}
