using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JoinConnector : MonoBehaviourPunCallbacks
{

    private void Awake()
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
}
