using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VotingManager : MonoBehaviourPunCallbacks
{
    private int votes = 0;
    private const int minVotes = 3;
    private int requiredVotes;

    [SerializeField]
    private Button voteButton;

    private GameplayController gameplayController;

    private void Awake()
    {
        gameplayController = GetComponent<GameplayController>();
        requiredVotes = 3;
    }
    
    public void OnVoteButton()
    {
        voteButton.gameObject.SetActive(false);
        photonView.RPC("VoteToStart", RpcTarget.All);
    }

    [PunRPC]
    void VoteToStart()
    {
        votes++;

        if (votes >= requiredVotes && votes >= minVotes)
        {
            //photonView.RPC("StartGame", RpcTarget.All);
            StartGame();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        ResetVoting();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        ResetVoting();
    }

    private void ResetVoting()
    {
        if (!gameplayController.isStarted)
        {
            requiredVotes = PhotonNetwork.CurrentRoom.PlayerCount;
            votes = 0;
            voteButton.gameObject.SetActive(true);
        }
    }

    private void StartGame()
    {
        Debug.Log("Game Starting!");
        gameplayController.StartGame();
    }
}
