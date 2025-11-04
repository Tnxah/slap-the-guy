using Photon.Pun;
using RockInMyShoe.Global.DataStorage;
using RockInMyShoe.Global.Eventing;
using UnityEngine.SceneManagement;

public class BattlegroundSceneEscape : MonoBehaviourPunCallbacks
{
    private PlayerControls playerControls;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Player.Back.performed += _ => Back();
    }

    private void Back()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
            PhotonNetwork.LeaveRoom();
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
