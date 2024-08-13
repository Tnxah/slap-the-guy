using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        if (photonView.IsMine)
        {
            playerControls = controller.playerControls;

            playerControls.Player.Attack.performed += _ => Attack();

            playerControls.Player.Throw.performed += _ => Throw();
        }
    }

    private void Attack()
    {
#if PLATFORM_ANDROID
        if (playerControls.TouchscreenHelper.Position.ReadValue<Vector2>().x < Screen.width / 2 || playerControls.TouchscreenHelper.Position.ReadValue<Vector2>().y > Screen.height / 2)
            return;
#endif

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

    private void Throw()
    {
#if PLATFORM_ANDROID
        if (playerControls.TouchscreenHelper.Position.ReadValue<Vector2>().x < Screen.width / 2 || playerControls.TouchscreenHelper.Position.ReadValue<Vector2>().y < Screen.height / 2)
            return;
#endif

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

        if (photonView.IsMine)
        {
            RoundStats.ItemThrown();
        }
    }

    public override void OnDisable()
    {
        if (photonView.IsMine)
        {
            playerControls.Player.Attack.performed -= _ => Attack();
            playerControls.Player.Throw.performed -= _ => Throw();
        }
    }
}
