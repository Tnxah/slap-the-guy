using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayController : MonoBehaviourPunCallbacks
{
    public static event Action onGameStart;
    public static event Action onGameEnd;
    public bool isStarted { private set; get; }

    private static int playerCount;

    private void FixedUpdate()
    {
        if (playerCount <= 1)
        {
            EndGame();
        }
    }

    public static void PlayerDies()
    {
        playerCount--;
    }

    public void StartGame()
    {
        if (!isStarted) { 
            isStarted = true;
            onGameStart?.Invoke();
        }

        playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
    }

    private void EndGame()
    {
        if (isStarted)
        {
            //isStarted = false;
            onGameEnd?.Invoke();
        }
    }

}
