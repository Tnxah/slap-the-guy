using Photon.Pun;
using Photon.Realtime;
using RockInMyShoe.Global.Eventing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VotingManager : MonoBehaviourPunCallbacks
{
    private int votes = 0;
    private int minVotes = 3;
    private int requiredVotes;
    private int autoVoteTime = 8;

    private bool botVoted = false;

    [SerializeField]
    private Button voteButton;
    [SerializeField]
    private TextMeshProUGUI votesText;
    [SerializeField]
    private TextMeshProUGUI readyText;
    [SerializeField]
    private TextMeshProUGUI autoVoteText;

    private Coroutine autoVoteRoutine;

    private GameplayController gameplayController;

    [SerializeField]
    private GameObject tutorialPanel; //TODO: REMOVE IT ITS TEMP SHIT!!!!!

    private void Awake()
    {
        gameplayController = GetComponent<GameplayController>();
        EventBus.Subscribe<OnRemoteConfigValuesFetched>(RetrieveMinPlayers, true);
        requiredVotes = minVotes;

        EventBus.Subscribe<OnCharacterCreatedEvent>(UpdateRequiredVotes);
    }
    
    public void OnVoteButton()
    {
        voteButton.gameObject.SetActive(false);
        StopCoroutine(autoVoteRoutine);
        autoVoteRoutine = null;

        autoVoteText.text = "READY";

        photonView.RPC("VoteToStart", RpcTarget.All);

        if (PhotonNetwork.IsMasterClient && (GameplayController.GetBotsAmount() > 0) && !botVoted)
        {
            for (int i = GameplayController.GetBotsAmount(); i > 0; i--)
            {
                photonView.RPC("VoteToStart", RpcTarget.All);
                print("BOT VOTED");
            }

            botVoted = true;
        }
    }

    [PunRPC]
    void VoteToStart()
    {
        votes++;
        SetVotesText();

        if (votes >= requiredVotes && votes >= minVotes)
        {
            StartCoroutine(StartGame());
        }
    }

    public void UpdateRequiredVotes(OnCharacterCreatedEvent evt)
    {
        photonView.RPC("RPCUpdateRequiredVotes", RpcTarget.All);
    }

    [PunRPC]
    void RPCUpdateRequiredVotes()
    {
        ResetVoting();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        ResetVoting();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ResetVoting();
    }

    public override void OnJoinedRoom()
    {
        ResetVoting();
    }

    private void ResetVoting()
    {
        if (!gameplayController.isStarted)
        {
            botVoted = false;

            requiredVotes = GetRequiredVotes();
            votes = 0;
            SetVotesText();
            voteButton.gameObject.SetActive(IsEnoughPlayers());

            ResetAutovoting();
        }
    }

    private void ResetAutovoting()
    {
        if(autoVoteRoutine != null)
            StopCoroutine(autoVoteRoutine);

        autoVoteRoutine = StartCoroutine(AutoVote());
    }

    private IEnumerator AutoVote()
    {
        for (int i = autoVoteTime; i >= 0; i--) {
            yield return new WaitUntil(() => !tutorialPanel.activeSelf);
            autoVoteText.text = $"READY ({i})";
            yield return new WaitForSeconds(1);
        }

        OnVoteButton();

        autoVoteText.text = "READY";
    }

    private void SetVotesText()
    {
        votesText.gameObject.SetActive(true);
        votesText.text = $"{votes}/{GetRequiredVotes()}";
    }

    private bool IsEnoughPlayers()
    {
        //return PhotonNetwork.CurrentRoom.PlayerCount >= minVotes;
        return GameplayController.PlayersWithBotsCount() >= minVotes;
    }

    private int GetRequiredVotes()
    {
        //print(PhotonNetwork.CurrentRoom.PlayerCount);
        print(GameplayController.PlayersWithBotsCount());
        print(minVotes);
        
        //return PhotonNetwork.CurrentRoom.PlayerCount > minVotes ? PhotonNetwork.CurrentRoom.PlayerCount : minVotes;
        return GameplayController.PlayersWithBotsCount() > minVotes ? GameplayController.PlayersWithBotsCount() : minVotes;
    }

    private IEnumerator StartGame()
    {
        Debug.Log("Game Starting!");
        EventBus.Publish(new OnAllVoted());
        votesText.gameObject.SetActive(false);

        for (int i = 0; i < 3; i++) {
            
            yield return new WaitForSeconds(1);
            
            switch (i) {
                case 0:
                    readyText.gameObject.SetActive(true);
                    readyText.text = "READY";
                    break;

                case 1:
                    readyText.text = "SET";
                    break;

                case 2:
                    readyText.text = "FIGHT";
                    break;
            }
        } 
        gameplayController.StartGame();

        yield return new WaitForSeconds(0.5f);

        readyText.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        //ResetVoting();
    }

    private void RetrieveMinPlayers(OnRemoteConfigValuesFetched evt)
    {
        minVotes = evt.appConfig.GetInt("MinAmountOfPlayers", minVotes);
        requiredVotes = PhotonNetwork.CurrentRoom == null ? minVotes : GetRequiredVotes();
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<OnRemoteConfigValuesFetched>(RetrieveMinPlayers);
        EventBus.Unsubscribe<OnCharacterCreatedEvent>(UpdateRequiredVotes);
    }

    public class OnCharacterCreatedEvent { }
    public class OnAllVoted { }
}
