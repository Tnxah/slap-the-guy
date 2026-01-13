using RockInMyShoe.Global.DataStorage;
using RockInMyShoe.Global.Eventing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndGameStatsUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI gameOverText;

    [SerializeField]
    private TextMeshProUGUI playersKnocked;
    [SerializeField]
    private TextMeshProUGUI damageDealt;
    [SerializeField]
    private TextMeshProUGUI damageReceived;
    [SerializeField]
    private TextMeshProUGUI itemsThrown;
    [SerializeField]
    private TextMeshProUGUI hits;
    [SerializeField]
    private TextMeshProUGUI timeInDodge;

    private void Awake()
    {
        gameObject.SetActive(false);

        EventBus.Subscribe<OnGameEndEvent>(EnableGameStats); //TODO move to some UIController and get rid of setactive false
        EventBus.Subscribe<BattleStatus>(SetBattleStatusText);
    }

    private void EnableGameStats(OnGameEndEvent evt)
    {
        gameObject.SetActive(true);
    }

    private void SetBattleStatusText(BattleStatus status)
    {
        gameOverText.text = status == BattleStatus.Win ? "YOU WIN!" : "Game Over";
    }

    private void RefreshStats()
    {
        playersKnocked.text = RoundStats.playersKnocked.ToString();
        damageDealt.text = RoundStats.damageDealt.ToString();
        damageReceived.text = RoundStats.damageReceived.ToString();
        itemsThrown.text = RoundStats.itemsThrown.ToString();
        hits.text = RoundStats.hits.ToString();
        timeInDodge.text = ((int)RoundStats.timeInDodge).ToString();

        RoundStats.ClearStats();
    }

    private void OnEnable()
    {
        RefreshStats();
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<OnGameEndEvent>(EnableGameStats);
    }
}
