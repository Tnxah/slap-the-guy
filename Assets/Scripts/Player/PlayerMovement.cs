using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class PlayerMovement : MonoBehaviourPunCallbacks, IPunObservable
{
    private PlayerController controller;
    private PlayerControls playerControls;

#if UNITY_ANDROID
    private const float InputDeadZone = 200f;
#else
    private const float InputDeadZone = 0.2f;
#endif
    private int direction;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();

        if ((photonView.IsRoomView && photonView.CompareTag("Player")))
        {
            controller.botAI.Rotate += Rotate;
        } 
        else if (photonView.IsMine)
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
        if (ctx.control?.device is Touchscreen ts)
        {
            if (ts.primaryTouch.position.ReadValue().x > Screen.width / 2)
                return;
        }

        var rawValue = ctx.ReadValue<float>();
        if (rawValue < InputDeadZone && rawValue > -InputDeadZone)
        { //handle dead zone
            return;
        }

        Rotate((int)rawValue);
    }

    private void Rotate(int rawValue)
    {
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
        if((photonView.IsRoomView && photonView.CompareTag("Player")))
            controller.botAI.Rotate -= Rotate;
        else if (photonView.IsMine)
            playerControls.Player.Rotate.performed -= ctx => Rotate(ctx);
        
    }
}
