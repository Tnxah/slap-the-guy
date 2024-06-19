using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviourPunCallbacks, IPunObservable
{
    private PlayerController controller;
    private PlayerControls playerControls;

    private int direction;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();

        if (photonView.IsMine)
        {
            playerControls = controller.playerControls;

            playerControls.Player.Rotate.performed += ctx => Rotate(ctx);
        }

        direction = (int)transform.lossyScale.x;
    }

    public int GetDirection()
    {
        return direction;
    }

    private void Rotate(InputAction.CallbackContext ctx)
    {
        photonView.RPC("PunRPC_Rotate", RpcTarget.All, (int)ctx.ReadValue<float>());
    }

    [PunRPC]
    private void PunRPC_Rotate(int direction)
    {
        this.direction = direction;
        SetScale(this.direction);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext((int)gameObject.transform.lossyScale.x);
        }
        else
        {
            var receivedDirection = (int)stream.ReceiveNext();
            this.direction = receivedDirection;
            SetScale(direction);
        }
    }

    private void SetScale(int direction)
    {
        gameObject.transform.localScale = new Vector3(direction, 1, 1);
    }
}
