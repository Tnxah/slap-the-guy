using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void OnFreePlay()
    {
        PlayerPrefs.SetInt("GameMode", 0); // 0 for Free Play
        SceneManager.LoadScene("Battleground");
    }

    public void OnMatchmaking()
    {
        PlayerPrefs.SetInt("GameMode", 1); // 1 for Matchmaking
        SceneManager.LoadScene("Battleground");
    }
}