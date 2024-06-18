using Photon.Pun;
using Photon.Pun.Demo.SlotRacer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;


public class PlayerCombat : MonoBehaviourPunCallbacks
{
    private PlayerController controller;
    private PlayerControls playerControls;

    private const int AttackCost = 15;
    private const int ThrowCost = 25;

    [SerializeField] //replace with Load from Recources
    private List<GameObject> throwablePrefabs = new List<GameObject>();
    
    [SerializeField]
    private Transform throwPoint;

    //For dynamic throwables
    private Random random = new Random();
    private int nextThrowable;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();

        if (photonView.IsMine)
        {
            playerControls = new PlayerControls();

            playerControls.Player.Attack.performed += _ => Attack();
            playerControls.Player.Throw.performed += _ => Throw();
        }
    }

    private void Attack()
    {
        if (controller.playerStats.TryUseStamina(AttackCost))
            photonView.RPC("PunRPC_Attack", RpcTarget.All);
    }

    [PunRPC]
    private void PunRPC_Attack()
    {
        controller.animationController.AttackAnimation();
    }

    private void Throw()
    {
        if (controller.playerStats.TryUseStamina(ThrowCost))
        {
            var randomId = random.Next(0, throwablePrefabs.Count);
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
        var throwableItem = Instantiate(throwablePrefabs[nextThrowable], throwPoint.position, Quaternion.identity).GetComponent<Rigidbody2D>();
        throwableItem.AddForce(new Vector2(-transform.localScale.x * 450, 0), ForceMode2D.Force);
    }

    public override void OnEnable()
    {
        base.OnEnable();

        if (photonView.IsMine)
        {
            playerControls.Player.Enable();
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();

        if (photonView.IsMine)
        {
            playerControls.Player.Disable();
        }
    }
}
