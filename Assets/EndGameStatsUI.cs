using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndGameStatsUI : MonoBehaviour
{
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

    private void RefreshStats()
    {
        playersKnocked.text = RoundStats.playersKnocked.ToString();
        damageDealt.text = RoundStats.damageDealt.ToString();
        damageReceived.text = RoundStats.damageReceived.ToString();
        itemsThrown.text = RoundStats.itemsThrown.ToString();
        hits.text = RoundStats.hits.ToString();
        timeInDodge.text = ((int)RoundStats.timeInDodge).ToString();
    }

    private void OnEnable()
    {
        RefreshStats();
    }
}
