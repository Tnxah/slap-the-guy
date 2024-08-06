using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviourPunCallbacks, IPunObservable
{
    private PlayerController controller;
    private PlayerControls playerControls;

#if PLATFORM_ANDROID
    private const float InputDeadZone = 200f;
#else
    private const float InputDeadZone = 0.2f;
#endif
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
#if PLATFORM_ANDROID
        if (playerControls.TouchscreenHelper.Position.ReadValue<Vector2>().x > Screen.width/ 2)
            return;
#endif

        var rawValue = ctx.ReadValue<float>();
        if (rawValue < InputDeadZone && rawValue > -InputDeadZone) { //handle dead zone
            return;
        }
        int value = (int)Mathf.Sign(rawValue);

        print("Rotate");
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
