using Photon.Pun;

public class PlayerDefense : MonoBehaviourPunCallbacks
{
    private PlayerController controller;
    private PlayerControls playerControls;

    private const int DodgeCost = 10;
    private const float DodgeStaminaBurn = 2.5f;

    private PlayerStats playerStats;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();

        if (photonView.IsMine)
        {
            playerControls = controller.playerControls;

            playerControls.Player.Dodge.performed += ctx => DodgeStart();
            playerControls.Player.Dodge.canceled += ctx => DodgeEnd();
        }

        playerStats = controller.playerStats;
    }

    private void DodgeStart()
    {
        if (playerStats.TryUseStamina(DodgeCost))
        {
            photonView.RPC("PunRPC_Dodge", RpcTarget.All, true);
            playerStats.StartStaminaBurn(DodgeStaminaBurn, DodgeEnd);
        }
    }

    private void DodgeEnd()
    {
        playerStats.StopStaminaBurn();
        photonView.RPC("PunRPC_Dodge", RpcTarget.All, false);
    }

    [PunRPC]
    private void PunRPC_Dodge(bool performing)
    {
        var animationController = controller.animationController;

        if (performing)
        {
            animationController.DodgeAnimation();
        }
        else
        {
            animationController.DodgeToIdleAnimation();
        }
    }
}
