using Photon.Pun;
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
}
