#if UNITY_ANDROID
using GooglePlayGames;
#endif
using Photon.Pun;
using Photon.Realtime;
using RockInMyShoe.Global.Eventing;
using TMPro;
using UnityEngine;
using static VotingManager;

public class NicknameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI nickname;

    private void Awake()
    {
        if (photonView.IsMine && !photonView.IsRoomView)
#if UNITY_EDITOR
            photonView.RPC("SetNicknameRPC", RpcTarget.All, "rockinmyshoe");
#elif UNITY_ANDROID
            photonView.RPC("SetNicknameRPC", RpcTarget.All, PlayGamesPlatform.Instance.GetUserDisplayName());
#elif UNITY_WEBGL
            HideNickname();
#endif
        else if (PhotonNetwork.IsMasterClient && photonView.IsRoomView && photonView.CompareTag("Player"))
        {
            photonView.RPC("SetNicknameRPC", RpcTarget.Others, "Bot Melman");
            SetNicknameRPC("Bot Melman");
        }

        EventBus.Subscribe<OnAllVoted>(_ => HideNickname());
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (photonView.IsMine && !photonView.IsRoomView)
            photonView.RPC("SetNicknameRPC", RpcTarget.All, PlayGamesPlatform.Instance.GetUserDisplayName());
        else if (PhotonNetwork.IsMasterClient && photonView.IsRoomView && photonView.CompareTag("Player"))
        {
            photonView.RPC("SetNicknameRPC", RpcTarget.Others, "Bot Melman");
            SetNicknameRPC("Bot Melman");
        }
    }

    [PunRPC]
    public void SetNicknameRPC(string name)
    {
        this.nickname.text = name;
    }

    private void HideNickname()
    {
        nickname.transform.parent.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<OnAllVoted>(_ => HideNickname());
    }
}
