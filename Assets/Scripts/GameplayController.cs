using Photon.Pun;
using System;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    public static event Action onGameStart;
    public bool isStarted { private set; get; }

    private RoomManager roomManager;


    public void StartGame()
    {
        if (!isStarted) { 
            isStarted = true;
            onGameStart?.Invoke();
        }
    }
}
