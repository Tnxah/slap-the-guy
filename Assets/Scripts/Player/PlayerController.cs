using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    //NEW========
    public AnimationController animationController;
    public PlayerStats playerStats;
    public PlayerCombat playerCombat;
    public PlayerMovement playerMovement;
    //NEW========

    private PlayerControls playerControls;


    private void Awake()
    {
        if (photonView.IsMine)
        {
            playerControls = new PlayerControls();

            playerControls.Player.Rotate.performed += direction => photonView.RPC("Rotate", RpcTarget.All, direction.ReadValue<float>());
            playerControls.Player.Dodge.performed += ctx => photonView.RPC("Dodge", RpcTarget.All, true);
            playerControls.Player.Dodge.canceled += ctx => photonView.RPC("Dodge", RpcTarget.All, false);
            playerControls.Player.Fake.performed += _ => Fake();
        }

        animationController = gameObject.GetComponent<AnimationController>();
        playerStats = gameObject.GetComponent<PlayerStats>();
        playerCombat = gameObject.GetComponent<PlayerCombat>();
        playerMovement = gameObject.GetComponent<PlayerMovement>();
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
