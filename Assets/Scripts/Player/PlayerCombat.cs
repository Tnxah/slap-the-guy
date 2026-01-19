using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = System.Random;


public class PlayerCombat : MonoBehaviourPunCallbacks
{
    private PlayerController controller;
    private PlayerControls playerControls;

    private const int AttackCost = 15;
    private const int ThrowCost = 20;

    [SerializeField] //replace with Load from Recources
    private List<GameObject> throwablePrefabs = new List<GameObject>();

    private List<int> availableThrowables = new List<int> { 0 };
    
    [SerializeField]
    private Transform throwPoint;
    private const float throwableSpeed = 450;

    //For dynamic throwables
    private Random random = new Random();
    private int nextThrowable;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();

        if ((photonView.IsRoomView && photonView.CompareTag("Player")))
        {
            controller.botAI.Attack += Attack;

            controller.botAI.Throw += Throw;
        }
        else if (photonView.IsMine)
        {
            playerControls = controller.playerControls;

            playerControls.Player.Attack.performed += ctx => Attack(ctx);

            playerControls.Player.Throw.performed += ctx => Throw(ctx);
        }
    }

    private void Attack(InputAction.CallbackContext ctx)
    {
        if(ctx.control?.device is Touchscreen ts)
        {
            if (ts.primaryTouch.position.ReadValue().x < Screen.width / 2 || ts.primaryTouch.position.ReadValue().y > Screen.height / 2) 
                return;
        }

        Attack();
    }

    private void Attack()
    {
        if (controller.playerStats.TryUseStamina(AttackCost))
        {
            print("Attack");
            photonView.RPC("PunRPC_Attack", RpcTarget.All);
        }
    }

    [PunRPC]
    private void PunRPC_Attack()
    {
        controller.animationController.AttackAnimation();
    }

    private void Throw(InputAction.CallbackContext ctx)
    {
        if (ctx.control?.device is Touchscreen ts)
        {
            if (ts.primaryTouch.position.ReadValue().x < Screen.width / 2 || ts.primaryTouch.position.ReadValue().y < Screen.height / 2)
                return;
        }

        Throw();
    }

    private void Throw()
    {
        if (controller.playerStats.TryUseStamina(ThrowCost))
        {
            var randomId = availableThrowables[random.Next(0, availableThrowables.Count)];
            photonView.RPC("PunRPC_Throw", RpcTarget.All, randomId);
        }
    }

    [PunRPC]
    private void PunRPC_Throw(int prefabId)
    {
        nextThrowable = prefabId;
        controller.animationController.ThrowAnimation();
    }

    private void InstantiateThrowable()
    {
        var throwableItem = Instantiate(throwablePrefabs[nextThrowable], 
            throwPoint.position, 
            Quaternion.identity).GetComponent<Rigidbody2D>();

        var direction = controller.playerMovement.GetDirection();

        throwableItem.gameObject.GetComponent<ThrownItem>().SetOwner(photonView);

        throwableItem.AddForce(new Vector2(direction * throwableSpeed, 0), ForceMode2D.Force);

        if (photonView.IsMine && !(photonView.IsRoomView && photonView.CompareTag("Player")))
        {
            RoundStats.ItemThrown();
        }
    }

    public override void OnDisable()
    {
        if ((photonView.IsRoomView && photonView.CompareTag("Player")))
        {
            controller.botAI.Attack -= Attack;
            controller.botAI.Throw -= Throw;
        }

        else if (photonView.IsMine)
        {
            playerControls.Player.Attack.performed -= ctx => Attack(ctx);
            playerControls.Player.Throw.performed -= ctx => Throw(ctx);
        }
    }
}
