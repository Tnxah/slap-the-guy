using Photon.Pun;
using Photon.Realtime;
using RockInMyShoe.Global.Eventing;
using TMPro;
using UnityEngine;

public class NicknameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TextMeshProUGUI nickname;

    private string nicknameName;

    private void Awake()
    {
        if (photonView.IsMine && !photonView.IsRoomView)
            photonView.RPC("SetNicknameRPC", RpcTarget.All, AuthenticationManager.instance.GetUserName());

        else if (PhotonNetwork.IsMasterClient && photonView.IsRoomView && photonView.CompareTag("Player"))
        {
            nicknameName = RandomNameGenerator.SingleTimeUseNickname();
            photonView.RPC("SetNicknameRPC", RpcTarget.Others, nicknameName);
            SetNicknameRPC(nicknameName);
        }

        //EventBus.Subscribe<OnAllVoted>(_ => HideNickname());
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (photonView.IsMine && !photonView.IsRoomView)
            photonView.RPC("SetNicknameRPC", RpcTarget.All, AuthenticationManager.instance.GetUserName());
        else if (PhotonNetwork.IsMasterClient && photonView.IsRoomView && photonView.CompareTag("Player"))
        {
            photonView.RPC("SetNicknameRPC", RpcTarget.Others, nicknameName);
            SetNicknameRPC(nicknameName);
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
        //EventBus.Unsubscribe<OnAllVoted>(_ => HideNickname());
    }
}
