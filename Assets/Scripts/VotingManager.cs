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
        requiredVotes = minVotes;
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

    public override void OnJoinedRoom()
    {
        ResetVoting();
    }

    private void ResetVoting()
    {
        if (!gameplayController.isStarted)
        {
            requiredVotes = GetRequiredVotes();
            votes = 0;
            voteButton.gameObject.SetActive(IsEnoughPlayers());
        }
    }

    private bool IsEnoughPlayers()
    {
        return PhotonNetwork.CurrentRoom.PlayerCount >= minVotes;
    }

    private int GetRequiredVotes()
    {
        return PhotonNetwork.CurrentRoom.PlayerCount >= 3 ? PhotonNetwork.CurrentRoom.PlayerCount : minVotes;
    }

    private void StartGame()
    {
        Debug.Log("Game Starting!");
        gameplayController.StartGame();
    }
}
