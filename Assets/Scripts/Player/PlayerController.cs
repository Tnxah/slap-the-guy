using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public AnimationController animationController;
    public PlayerStats playerStats;
    public PlayerCombat playerCombat;
    public PlayerMovement playerMovement;
    public PlayerControls playerControls { get; private set; }

    private void Awake()
    {
        if (photonView.IsMine)
        {
            playerControls = new PlayerControls();
        }

        animationController = gameObject.GetComponent<AnimationController>();
        playerStats = gameObject.GetComponent<PlayerStats>();
        playerCombat = gameObject.GetComponent<PlayerCombat>();
        playerMovement = gameObject.GetComponent<PlayerMovement>();

        GameplayController.onGameStart += OnGameStart;
    }

    private void OnGameStart()
    {
        EnableAttack();
    }

    public void EnableAttack()
    {
        if (photonView.IsMine)
        {
            playerControls.Player.Attack.Enable();
            playerControls.Player.Throw.Enable();
            playerControls.Player.Dodge.Enable();
        }
    }

    public void DisableAttack()
    {
        if (photonView.IsMine)
        {
            playerControls.Player.Attack.Disable();
            playerControls.Player.Throw.Disable();
            playerControls.Player.Dodge.Disable();
        }
    }

    public void EnableControls()
    {
        if (photonView.IsMine)
        {
            playerControls.Player.Enable();
        }
    }

    public void DisableControls()
    {
        if (photonView.IsMine)
        {
            playerControls.Player.Disable();
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        EnableControls(); //not sure about enabling from player appear
                            //maybe should wait until game start
        DisableAttack();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        DisableControls();
    }
}
