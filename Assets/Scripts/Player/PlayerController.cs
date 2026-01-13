using Photon.Pun;
using Photon.Realtime;
using RockInMyShoe.Global.Eventing;
using UnityEngine;
using static VotingManager;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public AnimationController animationController;
    public PlayerStats playerStats;
    public PlayerCombat playerCombat;
    public PlayerMovement playerMovement;
    public PlayerControls playerControls { get; private set; }
    public PlayerSoundController playerSoundController;

    [SerializeField]
    private GameObject pointer;

    public SimpleBotAI botAI;

    private void Awake()
    {
        if (!(photonView.IsRoomView && photonView.CompareTag("Player")))
        {
            botAI.enabled = false;
        }

        if (photonView.IsMine)
        {
            playerControls = new PlayerControls();
            GameplayController.onGameStart += OnGameStart;
            EventBus.Subscribe<OnGameEndEvent>(OnGameEnd);
            if(!(photonView.IsRoomView && photonView.CompareTag("Player")))
                pointer.SetActive(true);
        }

        if ((photonView.IsRoomView && photonView.CompareTag("Player")))
        {
            EventBus.Publish<OnCharacterCreatedEvent>(new OnCharacterCreatedEvent());
            GameplayController.onGameStart += botAI.OnGameStart;
        }

        animationController = gameObject.GetComponent<AnimationController>();
        playerStats = gameObject.GetComponent<PlayerStats>();
        playerCombat = gameObject.GetComponent<PlayerCombat>();
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        playerSoundController = gameObject.GetComponent<PlayerSoundController>();
    }

    private void OnGameStart()
    {
        if(photonView.IsMine)
            EnableAttack();
    }
    private void OnGameEnd(OnGameEndEvent evt)
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
#if PLATFORM_ANDROID
            playerControls.TouchscreenHelper.Enable();
#endif
        }
    }

    public void DisableControls()
    {
        if (photonView.IsMine)
        {
            playerControls.Player.Disable();
#if PLATFORM_ANDROID
            playerControls.TouchscreenHelper.Disable();
#endif
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
            EventBus.Unsubscribe<OnGameEndEvent>(OnGameEnd);
        }
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if ((photonView.IsRoomView && photonView.CompareTag("Player")) && GameplayController.PlayersWithBotsCount() > 4)
        {
            PhotonNetwork.Destroy(photonView);
            EventBus.Publish<OnCharacterCreatedEvent>(new OnCharacterCreatedEvent());
        }
    }
}
