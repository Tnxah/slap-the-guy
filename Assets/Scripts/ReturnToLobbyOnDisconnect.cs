using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToLobbyOnDisconnect : MonoBehaviourPunCallbacks
{
    private bool returning;

    [SerializeField]
    private TextMeshProUGUI text;

    private void Start()
    {
        PhotonNetwork.KeepAliveInBackground = 120;
    }

    void OnApplicationPause(bool paused)
    {
        if (paused)
        {
        }
        else
        {
            StartCoroutine(CheckingTheConnection());
        }
    }    

    private IEnumerator CheckingTheConnection()
    {
        text.text = "Checking the connection...";

        yield return new WaitForSeconds(4);

        text.text = "";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        StartCoroutine(BeginReturnToLobby($"Disconnected: {cause}"));
    }

    private IEnumerator BeginReturnToLobby(string reason)
    {
        if (returning) yield return null;
        returning = true;

        text.text = "Connection lost... Returning to lobby.";

        yield return new WaitForSeconds(3);

        Debug.Log($"ReturnToLobby triggered. Reason: {reason}");

        // Stop Photon from thinking we're in a room locally
        // Destroy local network objects to avoid "ghost player"
        if (PhotonNetwork.InRoom)
        {
            // This destroys YOUR instantiated networked objects locally too
            PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
            PhotonNetwork.LeaveRoom();
        }

        // Ensure we are fully disconnected (idempotent)
        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();

        yield return new WaitUntil (() => (!PhotonNetwork.InRoom && !PhotonNetwork.IsConnected));

        SceneManager.LoadScene("Lobby");

        text.text = "";
    }
}
