using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    private AnimationController animationController;
    private PlayerStats playerStats;
    private PlayerControls playerControls;

    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private GameObject throwablePrefab;

    [SerializeField]
    private Transform throwPoint;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            playerControls = new PlayerControls();

            playerControls.Player.Rotate.performed += direction => photonView.RPC("Rotate", RpcTarget.All, direction.ReadValue<float>());
            playerControls.Player.Attack.performed += _ => photonView.RPC("Attack", RpcTarget.All);
            playerControls.Player.Dodge.performed += ctx => photonView.RPC("Dodge", RpcTarget.All, true);
            playerControls.Player.Dodge.canceled += ctx => photonView.RPC("Dodge", RpcTarget.All, false);
            playerControls.Player.Throw.performed += _ => photonView.RPC("Throw", RpcTarget.All); ;
            playerControls.Player.Fake.performed += _ => Fake();
        }

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        animationController = gameObject.GetComponent<AnimationController>();
        playerStats = gameObject.GetComponent<PlayerStats>();
    }

    [PunRPC]
    private void Attack()
    {
        if(playerStats.TryUseStamina(20))
        animationController.AttackAnimation();
    }

    [PunRPC]
    private void Dodge(bool perform)
    {
        if (perform && playerStats.StartStaminaBurn(10, () => photonView.RPC("Dodge", RpcTarget.All, false)))
        {
            animationController.DodgeAnimation();
        }
        else 
        {
            playerStats.StopStaminaBurn();
            animationController.DodgeToIdleAnimation();
        }
    }

    [PunRPC]
    private void Throw()
    {
        if (playerStats.TryUseStamina(30))
            animationController.ThrowAnimation();
    }

    private void ThrowItem()
    {
        var throwableItem = Instantiate(throwablePrefab, throwPoint.position, Quaternion.identity).GetComponent<Rigidbody2D>();
        throwableItem.AddForce(new Vector2(-transform.localScale.x * 450, 0), ForceMode2D.Force);
    }

    [PunRPC]
    private void Fake()
    {

    }

    [PunRPC]
    private void Rotate(float direction)
    {
        //spriteRenderer.flipX = direction > 0f ? true : false;
        var newScale = direction > 0f ? new Vector3(-1,1,1) : new Vector3(1,1,1);
        SetScale(newScale);
    }

    private void SetScale(Vector3 scale)
    {
        gameObject.transform.localScale = scale;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(gameObject.transform.localScale);
        }
        else
        {
            var dir = (Vector3)stream.ReceiveNext();
            // Network player, receive data
            SetScale(dir);
        }
    }

    private void OnEnable()
    {
        if (photonView.IsMine)
        {
            playerControls.Player.Enable();
        }
    }

    private void OnDisable()
    {
        if (photonView.IsMine)
        {
            playerControls.Player.Disable();
        }
    }
}
