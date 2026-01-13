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

    private bool botVoted = false;

    [SerializeField]
    private Button voteButton;
    [SerializeField]
    private TextMeshProUGUI votesText;
    [SerializeField]
    private TextMeshProUGUI readyText;

    private GameplayController gameplayController;

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
        photonView.RPC("VoteToStart", RpcTarget.All);

        print("BOTS" + GameplayController.GetBotsAmount());
        print(PhotonNetwork.IsMasterClient + " " + (GameplayController.GetBotsAmount() > 0) + " " + !botVoted);
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
        }
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

        yield return new WaitForSeconds(1);

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
