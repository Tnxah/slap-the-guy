using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayController : MonoBehaviourPunCallbacks
{
    public static event Action onGameStart;
    public bool isStarted { private set; get; }

    private RoomManager roomManager;

    private void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            int gameMode = PlayerPrefs.GetInt("GameMode");
            if (gameMode == 0)
            {
                string roomName = PlayerPrefs.GetString("RoomName", null);
                if (!string.IsNullOrEmpty(roomName))
                {
                    PhotonNetwork.JoinRoom(roomName);
                }
                else
                {
                    Debug.LogError("No room name found. Returning to Lobby.");
                    SceneManager.LoadScene("Lobby");
                }
            }
            else { SceneManager.LoadScene("Lobby"); }
        }
    }

    public void StartGame()
    {
        if (!isStarted) { 
            isStarted = true;
            onGameStart?.Invoke();
        }
    }
}
