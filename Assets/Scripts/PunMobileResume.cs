using Photon.Pun;
using Photon.Realtime;
using RockInMyShoe.Global.DataStorage;
using RockInMyShoe.Global.Eventing;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PunMobileResume : MonoBehaviourPunCallbacks
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        
    }

    void OnApplicationPause(bool paused)
    {
        if (!paused)
        {
            // Coming back:
            if (!PhotonNetwork.IsConnected)
            {
                var status = StatusStorage.GetStatus<BattleStatus>();
                EventBus.Publish(new BackToLobbyEvent { status = status });
                print("BackToLobbyEvent" + status);
                SceneManager.LoadScene("Lobby");
            }
        }
    }


}
