using Photon.Pun;
using RockInMyShoe.Global.DataStorage;
using RockInMyShoe.Global.Eventing;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattlegroundSceneEscape : MonoBehaviourPunCallbacks
{
    private PlayerControls playerControls;

    private bool isBacking = false;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Player.Back.performed += _ => StartCoroutine(Back());
    }

    public void ButtonBack()
    {
        StartCoroutine(Back());
    }

    private IEnumerator Back()
    {
        if (isBacking) yield return null;

        isBacking = true;

        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }
        else
        {
            PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
        }

        yield return new WaitUntil(() => (PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Disconnected || PhotonNetwork.IsConnectedAndReady));

        if (PhotonNetwork.IsConnectedAndReady || PhotonNetwork.NetworkClientState == Photon.Realtime.ClientState.Disconnected)
        {
            var status = StatusStorage.GetStatus<BattleStatus>();
            EventBus.Publish(new BackToLobbyEvent { status =  status});
            print("BackToLobbyEvent" + status);
            SceneManager.LoadScene("Lobby");
        }
    }

    public override void OnEnable()
    {
        playerControls.Player.Back.Enable();
    }

    public override void OnDisable()
    {
        playerControls.Player.Back.Disable();
    }

    private void OnApplicationQuit()
    {
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
        PhotonNetwork.LeaveRoom();
    }
}

public sealed class BackToLobbyEvent { 
    public BattleStatus status;
}

public enum BattleStatus
{
    None,
    Win,
    Lose
}
