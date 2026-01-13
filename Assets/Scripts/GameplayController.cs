using Photon.Pun;
using RockInMyShoe.Global.DataStorage;
using RockInMyShoe.Global.Eventing;
using System;
using UnityEngine;

public class GameplayController : MonoBehaviourPunCallbacks
{
    public static event Action onGameStart;
    public bool isStarted { private set; get; }

    private static int playerCount;

    private void Awake()
    {
        EventBus.Subscribe<OnPlayerDieEvent>(CheckEndGame);
    }

    public static void PlayerDies()
    {
        playerCount--;
        EventBus.Publish(new OnPlayerDieEvent { });
    }

    public void StartGame()
    {
        if (!isStarted) { 
            isStarted = true;
            onGameStart?.Invoke();
        }

        StatusStorage.SetStatus(BattleStatus.Lose);
        playerCount = PlayersWithBotsCount();
    }

    private void CheckEndGame(OnPlayerDieEvent evt)
    {
        print($"CheckEndGame {isStarted} {playerCount}");
        if (isStarted && playerCount <= 1)
        {
            //isStarted = false;
            print(StatusStorage.GetStatus<BattleStatus>());
            EventBus.Publish(new OnGameEndEvent());
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<OnPlayerDieEvent>(CheckEndGame);
    }

    public static int PlayersWithBotsCount()
    {
        var count = PhotonNetwork.CurrentRoom.PlayerCount;

        count += GetBotsAmount();

        return count;
    }

    public static int GetBotsAmount()
    {
        int count = 0;

        var views = FindObjectsByType<PhotonView>(FindObjectsSortMode.None);
        foreach (var view in views)
        {
            if (view.IsRoomView && view.CompareTag("Player"))
            {
                print(view.gameObject.name);
                count++;
            }
        }

        return count;
    }
}

public class OnGameEndEvent { }
