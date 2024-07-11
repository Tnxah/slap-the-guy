using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public AnimationController animationController;
    public PlayerStats playerStats;
    public PlayerCombat playerCombat;
    public PlayerMovement playerMovement;
    public PlayerControls playerControls { get; private set; }

    [SerializeField]
    private GameObject pointer;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            playerControls = new PlayerControls();
            GameplayController.onGameStart += OnGameStart;
            GameplayController.onGameEnd += OnGameEnd;
            pointer.SetActive(true);
        }

        animationController = gameObject.GetComponent<AnimationController>();
        playerStats = gameObject.GetComponent<PlayerStats>();
        playerCombat = gameObject.GetComponent<PlayerCombat>();
        playerMovement = gameObject.GetComponent<PlayerMovement>();

    }

    private void OnGameStart()
    {
        if(photonView.IsMine)
            EnableAttack();
    }
    private void OnGameEnd()
    {
        if(photonView.IsMine)
            DisableAttack();
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
        EnableControls();    
        DisableAttack();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        DisableControls();

        if (photonView.IsMine)
        {
            GameplayController.onGameStart -= OnGameStart;
            GameplayController.onGameEnd -= OnGameEnd;
        }
    }
}
