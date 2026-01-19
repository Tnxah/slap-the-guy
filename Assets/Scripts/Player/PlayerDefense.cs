using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDefense : MonoBehaviourPunCallbacks
{
    private PlayerController controller;
    private PlayerControls playerControls;

    private const int DodgeCost = 8;
    private const float DodgeStaminaBurn = 7f;

    private PlayerStats playerStats;

    //====Tracking for RoundStats====
    private float dodgeStartTime; 
    private bool dodging;
    //===============================


    private void Awake()
    {
        controller = GetComponent<PlayerController>();

        if ((photonView.IsRoomView && photonView.CompareTag("Player")))
        {
            controller.botAI.DodgeStart += DodgeStart;
            controller.botAI.DodgeEnd += DodgeEnd;
        }
        else if (photonView.IsMine)
        {
            playerControls = controller.playerControls;

            playerControls.Player.Dodge.performed += ctx => DodgeStart(ctx);
            playerControls.Player.Dodge.canceled += ctx => DodgeEnd();
        }

        playerStats = controller.playerStats;
    }

    private void DodgeStart(InputAction.CallbackContext ctx)
    {
        if (ctx.control?.device is Touchscreen ts)
        {
            StartCoroutine(TouchscreenDodgeStart(ts));
            return;
        }
        else
        {
            DodgeStart();
        }
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

    private IEnumerator TouchscreenDodgeStart(Touchscreen ts)
    {   var startPos = ts.primaryTouch.position.ReadValue().y;

        yield return new WaitForSeconds(0.1f);

        var direction = startPos - ts.primaryTouch.position.ReadValue().y;

        if (direction > 20 && playerControls.TouchscreenHelper.Position.ReadValue<Vector2>().x < Screen.width / 2) 
        {
            print("TouchscreenDodgeStart");
            DodgeStart();
        }
    }

    private void DodgeEnd()
    {
        playerStats.StopStaminaBurn();

        if (photonView.IsMine && !(photonView.IsRoomView && photonView.CompareTag("Player")) && dodging)
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
        if ((photonView.IsRoomView && photonView.CompareTag("Player")))
        {
            controller.botAI.DodgeStart -= DodgeStart;
            controller.botAI.DodgeEnd -= DodgeEnd;
        }

        else if (photonView.IsMine)
        {
            playerControls.Player.Dodge.performed -= ctx => DodgeStart(ctx);
            playerControls.Player.Dodge.canceled -= ctx => DodgeEnd();
        }
    }
}
