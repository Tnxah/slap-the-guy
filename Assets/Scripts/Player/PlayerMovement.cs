using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviourPunCallbacks, IPunObservable
{
    private PlayerController controller;
    private PlayerControls playerControls;

    private const float ControllerDeadZone = 0.2f;

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
        var rawValue = ctx.ReadValue<float>();

        if (rawValue < ControllerDeadZone && rawValue > -ControllerDeadZone) { //handle controller dead zone
            return;
        }
        int value = (int)Mathf.Sign(rawValue);

        photonView.RPC("PunRPC_Rotate", RpcTarget.All, value);
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

    public override void OnDisable()
    {
        if (photonView.IsMine)
            playerControls.Player.Rotate.performed -= ctx => Rotate(ctx);
    }
}
