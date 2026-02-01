using Photon.Pun;
using Photon.Realtime;
using RockInMyShoe.Global.DataStorage;
using RockInMyShoe.Global.Eventing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static VotingManager;

public class RoomManager : MonoBehaviourPunCallbacks
{
    private GameplayController gameplayController;

    private Dictionary<Player, Transform> playerSpawnPoints = new Dictionary<Player, Transform>();

    [SerializeField]
    private List<Transform> spawnPoints;

    [SerializeField]
    private TextMeshProUGUI roomNameGraffiti;

    private string lastRoomName;
    private Coroutine rematchRoutine;
    private void Awake()
    {
        gameplayController = GetComponent<GameplayController>();
        GameplayController.onGameStart += CloseRoom;
        EventBus.Subscribe<OnAllVoted>(CloseRoom);
        EventBus.Subscribe<OnGameEndEvent>(LeaveRoom);
    }

    private void CreatePlayer()
    {
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player entered: " + newPlayer.NickName);
        OnPlayerEnteredOrLeft();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("Player left: " + otherPlayer.NickName);
        
        PhotonNetwork.DestroyPlayerObjects(otherPlayer);

        GameplayController.PlayerDies();
        OnPlayerEnteredOrLeft();
    }

    [PunRPC]
    private void UpdatePlayerListAndSpawnPoints()
    {
        if (gameplayController.isStarted) return;

        List<Player> sortedPlayers = PhotonNetwork.PlayerList.OrderBy(p => p.ActorNumber).ToList();

        playerSpawnPoints.Clear();

        PhotonView[] photonViews = FindObjectsByType<PhotonView>(FindObjectsSortMode.InstanceID);
        var currentBotnumber = 0;
        foreach (PhotonView view in photonViews)
        {
            if (view.IsRoomView && view.CompareTag("Player"))
            {
                print($"Player COUNT = {PhotonNetwork.PlayerList.Count()} current bot number = {currentBotnumber}; mesto bota = {currentBotnumber}");
                view.transform.position = spawnPoints[currentBotnumber].position;
                currentBotnumber++;
            }
        }


        for (int i = 0; i < sortedPlayers.Count; i++)
        {
            if (i < spawnPoints.Count)
            {
                print($"mesto igroka = {i} igrok = {sortedPlayers[i].ActorNumber}");
                playerSpawnPoints[sortedPlayers[i]] = spawnPoints[i + currentBotnumber];
                MovePlayerToSpawnPoint(sortedPlayers[i], spawnPoints[i + currentBotnumber]);
            }
        }

        //======================================

        //PhotonView[] photonViews = FindObjectsByType<PhotonView>(FindObjectsSortMode.InstanceID);
        //var players = 0;
        //print($"photonViewsCount: {photonViews.Length}");
        //foreach (PhotonView view in photonViews)
        //{
        //    print($"{view.name} / {view.InstantiationId}");
        //    if (view.CompareTag("Player"))
        //    {
        //        print($"PLAYER!!!! {view.name} / {view.InstantiationId} place: {spawnPoints[players].position} playernumber {players}");
        //        //print($"Player COUNT = {PhotonNetwork.PlayerList.Count()} current bot number = {currentBotnumber}; mesto bota = {PhotonNetwork.PlayerList.Count() + currentBotnumber}");
        //        view.transform.position = spawnPoints[players].position;
        //        players++;
        //    }
        //}

        //======================================
    }

    private void MovePlayerToSpawnPoint(Player player, Transform spawnPoint)
    {
        PhotonView[] photonViews = FindObjectsByType<PhotonView>(FindObjectsSortMode.InstanceID);
        foreach (PhotonView view in photonViews)
        {
            if (view.Owner == player && view.CompareTag("Player") && !view.IsRoomView)
            {
                print(view.Owner.ActorNumber + " oaoaoao " + player.ActorNumber + " --- " + spawnPoint.name);
                view.transform.position = spawnPoint.position;

                print("player pos is now = " + view.transform.position);
                return;
            }
        }
    }

    private void CheckRoomAvailability()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom && !gameplayController.isStarted)
        {
            if (GameplayController.PlayersWithBotsCount() >= PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                CloseRoom();
            }
            else
            {
                OpenRoom();
            }
        }
    }

    private void OnPlayerEnteredOrLeft()
    {
        photonView.RPC("UpdatePlayerListAndSpawnPoints", RpcTarget.All);
        CheckRoomAvailability();
    }

    private void CloseRoom()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
        {
            Debug.Log("Room Closed");
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
    }

    private void CloseRoom(OnAllVoted evt)
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
        {
            Debug.Log("Room Closed");
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
    }

    private  void OpenRoom()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
        {
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        lastRoomName = PhotonNetwork.CurrentRoom.Name;
        roomNameGraffiti.text = PhotonNetwork.CurrentRoom.Name;

        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < 2; i++)
            {
                PhotonNetwork.InstantiateRoomObject("Player", Vector3.zero, Quaternion.identity);
            }
        }
            

        CreatePlayer();
        OnPlayerEnteredOrLeft();

        StatusStorage.SetStatus(BattleStatus.None);
    }

    public void Replay() //TODO: THIS IS TEMP! REMOVE!!!
    {
        if (rematchRoutine != null) return;

        rematchRoutine = StartCoroutine(ReplayRoutine());
    }

    private IEnumerator ReplayRoutine()
    {
        if(PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();

        yield return new WaitUntil(() =>
        PhotonNetwork.IsConnected &&
        PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer
    );

        if (!string.IsNullOrEmpty(lastRoomName))
        {
            if (PhotonNetwork.JoinOrCreateRoom(lastRoomName, new RoomOptions { MaxPlayers = 5 }, TypedLobby.Default))
                SceneManager.LoadScene("Battleground");
        }
        else
        {
            SceneManager.LoadScene("Lobby");
        }

        rematchRoutine = null;
    }

    private void LeaveRoom(OnGameEndEvent evt)
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
            Debug.Log("Something went wrong. Create your own room");
        //PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayers });
        PhotonNetwork.JoinOrCreateRoom(AuthenticationManager.instance.GetUserName(), new RoomOptions { MaxPlayers = 5 }, TypedLobby.Default);
                
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<OnAllVoted>(CloseRoom);
        EventBus.Unsubscribe<OnGameEndEvent>(LeaveRoom);
    }
}
