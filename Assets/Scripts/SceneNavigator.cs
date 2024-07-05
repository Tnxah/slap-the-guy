using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigator : MonoBehaviourPunCallbacks
{
    private PlayerControls playerControls;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        playerControls = new PlayerControls();
        playerControls.Player.Back.performed += _ => Back();
    }

    private void Back()
    {
        if (SceneManager.GetActiveScene().buildIndex > 0)
        {
            if (SceneManager.GetActiveScene().Equals("Battleground"))
                PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
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
