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

    public Button voteButton;

    private void Start()
    {
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
            photonView.RPC("StartGame", RpcTarget.All);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        requiredVotes = PhotonNetwork.CurrentRoom.PlayerCount;
        votes = 0;
        ResetVoteButton();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        requiredVotes = PhotonNetwork.CurrentRoom.PlayerCount;
        votes = 0;
        ResetVoteButton();
    }

    private void ResetVoteButton()
    {
        if (!GameplayController.isStarted)
        // Reset the vote button for all players
        voteButton.gameObject.SetActive(true);
    }

    [PunRPC]
    void StartGame()
    {
        Debug.Log("Game Starting!");
        GameplayController.StartGame();
    }
}
