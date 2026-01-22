using RockInMyShoe.Global.Eventing;
using System;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instanse;
    private int _adsCooldown = 3;

    private IAdsProvider _provider;

    private static int currentCount;

    private bool _initialized;

    private void Awake()
    {
        if(Instanse == null)
        {
            Instanse = this;
            EventBus.Subscribe<OnRemoteConfigValuesFetched>(RetrieveAdsCooldown);
        } else
        {
            Destroy(gameObject);
        }

        _provider = new LevelPlayAdsProvider("23ff76ccd", "4n2nznmhl8jyr3f6", "ag5z1prg1ezctjf4");
    }

    private async void Start()
    {
        var result = await _provider.InitializeAsync();

        if (result.Success)
            OnInitSuccess();
        else
            OnInitFailed(result.Error);

    }

    private void ShowAdOnCountdown(BackToLobbyEvent evt)
    {
        if (evt.status != BattleStatus.Lose)
            return;

        currentCount++;

        if (currentCount >= _adsCooldown)
        {
            _provider.TryShowInterstitial();
            currentCount = 0;
        }
    }

    private void ShowInterstitial()
    {
        _provider.TryShowInterstitial();
    }

    private void ShowRewarded(Action onRewarded = null)
    {
        _provider.TryShowRewarded(onRewarded);
    }

    private void OnInitSuccess()
    {
        EventBus.Subscribe<BackToLobbyEvent>(ShowAdOnCountdown);
        _initialized = true;
    }

    private void OnInitFailed(string error)
    {
        Debug.LogError($"Init failed: {error}");
    }

    private void RetrieveAdsCooldown(OnRemoteConfigValuesFetched evt)
    {
        _adsCooldown = evt.appConfig.GetInt("AdsCooldown", _adsCooldown);
    }

    private void OnEnable()
    {
        if(!_initialized)
            EventBus.Subscribe<BackToLobbyEvent>(ShowAdOnCountdown);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<BackToLobbyEvent>(ShowAdOnCountdown);
    }
}
