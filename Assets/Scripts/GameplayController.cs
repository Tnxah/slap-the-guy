using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    public static event Action onGameStart;
    public static bool isStarted { private set; get; }


    public static void StartGame()
    {
        if (!isStarted) { 
            isStarted = true;
            if (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom && !GameplayController.isStarted)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;  // Close the room
                PhotonNetwork.CurrentRoom.IsVisible = false;  // Hide the room from the list
            }
            onGameStart?.Invoke();
        }
    }
}
