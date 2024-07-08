using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VotingManager : MonoBehaviourPunCallbacks
{
    private int votes = 0;
    private const int minVotes = 3;
    private int requiredVotes;

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
        SetVotesText();

        if (votes >= requiredVotes && votes >= minVotes)
        {
            StartCoroutine(StartGame());
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
        return PhotonNetwork.CurrentRoom.PlayerCount >= minVotes;
    }

    private int GetRequiredVotes()
    {
        return PhotonNetwork.CurrentRoom.PlayerCount >= 3 ? PhotonNetwork.CurrentRoom.PlayerCount : minVotes;
    }

    private IEnumerator StartGame()
    {
        Debug.Log("Game Starting!");
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
}
