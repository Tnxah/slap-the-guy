using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDefense : MonoBehaviourPunCallbacks
{
    private PlayerController controller;
    private PlayerControls playerControls;

    private const int DodgeCost = 10;
    private const float DodgeStaminaBurn = 8f;

    private PlayerStats playerStats;

    //====Tracking for RoundStats====
    private float dodgeStartTime; 
    private bool dodging;
    //===============================


    private void Awake()
    {
        controller = GetComponent<PlayerController>();

        if (photonView.IsMine)
        {
            playerControls = controller.playerControls;

#if PLATFORM_ANDROID
            playerControls.Player.Dodge.performed += ctx => StartCoroutine(TouchscreenDodgeStart());
#else 
            playerControls.Player.Dodge.performed += ctx => DodgeStart();
#endif
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
            
            if(photonView.IsMine)
                dodgeStartTime = Time.time;
        }
    }

    private IEnumerator TouchscreenDodgeStart()
    {
        var startPos = playerControls.TouchscreenHelper.Position.ReadValue<Vector2>().y;

        yield return new WaitForSeconds(0.1f);

        var direction = startPos - playerControls.TouchscreenHelper.Position.ReadValue<Vector2>().y;

        if (direction > 20 && playerControls.TouchscreenHelper.Position.ReadValue<Vector2>().x < Screen.width / 2) 
        {
            print("TouchscreenDodgeStart");
            DodgeStart();
        }
    }

    private void DodgeEnd()
    {
        playerStats.StopStaminaBurn();

        if (photonView.IsMine)
        {
            RoundStats.DodgedSeconds(Time.time - dodgeStartTime);
        }

        photonView.RPC("PunRPC_Dodge", RpcTarget.All, false);

    }

    [PunRPC]
    private void PunRPC_Dodge(bool performing)
    {
        dodging = performing;

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

    public override void OnDisable()
    {
        if (photonView.IsMine)
        {
            playerControls.Player.Dodge.performed -= ctx => DodgeStart();
            playerControls.Player.Dodge.canceled -= ctx => DodgeEnd();
        }
    }
}
