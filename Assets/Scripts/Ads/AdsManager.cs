using RockInMyShoe.Global.Eventing;
using Unity.Services.LevelPlay;
using Unity.Services.RemoteConfig;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instanse;

    private InterstitialAdManager InterstitialAd { get; set; }
    private LevelPlayRewardedAd RewardedAd { get; set; }
    private int _adsCooldown = 3;

    //----------------

    private static int currentCount;

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
    }

    public void Start()
    {
        LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
        LevelPlay.OnInitFailed += SdkInitializationFailedEvent;

        // SDK init
        LevelPlay.Init("23ff76ccd");
    }

    private void InitializeRewardedAd()
    {
        if (RewardedAd == null)
        {
            RewardedAd = new LevelPlayRewardedAd("4n2nznmhl8jyr3f6");
            PrepareRewarded();
        }

        RewardedAd.LoadAd();
    }

    private void SdkInitializationFailedEvent(LevelPlayInitError error) { }

    private void SdkInitializationCompletedEvent(LevelPlayConfiguration configuration)
    {
        //LevelPlay.LaunchTestSuite();
        InitializeRewardedAd();
        InterstitialAd = new();

        EventBus.Subscribe<BackToLobbyEvent>(ShowAdOnCountdown);
    }


    private void ShowAdOnCountdown(BackToLobbyEvent evt)
    {
        print($"{currentCount} / {_adsCooldown} - {evt.status}");

        if (evt.status != BattleStatus.Lose)
            return;

        currentCount++;

        
        if (currentCount >= _adsCooldown)
        {
            InterstitialAd.ShowAd();

            currentCount = 0;
        }
    }

    private void PrepareRewarded()
    {
        // Register to Rewarded events
        RewardedAd.OnAdLoaded += RewardedOnAdLoadedEvent;
        RewardedAd.OnAdLoadFailed += RewardedOnAdLoadFailedEvent;
        RewardedAd.OnAdDisplayed += RewardedOnAdDisplayedEvent;
        RewardedAd.OnAdDisplayFailed += RewardedOnAdDisplayFailedEvent;
        RewardedAd.OnAdRewarded += RewardedOnAdRewardedEvent;
        RewardedAd.OnAdClosed += RewardedOnAdClosedEvent;
        // Optional 
        RewardedAd.OnAdClicked += RewardedOnAdClickedEvent;
        RewardedAd.OnAdInfoChanged += RewardedOnAdInfoChangedEvent;

        // Implement the events
        void RewardedOnAdLoadedEvent(LevelPlayAdInfo adInfo) { }
        void RewardedOnAdLoadFailedEvent(LevelPlayAdError error) { RewardedAd.LoadAd();  }
        void RewardedOnAdDisplayedEvent(LevelPlayAdInfo adInfo) { RewardedAd.LoadAd(); }
        void RewardedOnAdDisplayFailedEvent(LevelPlayAdInfo adInfo, LevelPlayAdError error) { RewardedAd.LoadAd(); }
        void RewardedOnAdRewardedEvent(LevelPlayAdInfo adInfo, LevelPlayReward adReward) { RewardedAd.LoadAd(); }
        void RewardedOnAdClosedEvent(LevelPlayAdInfo adInfo) { RewardedAd.LoadAd(); }
        void RewardedOnAdClickedEvent(LevelPlayAdInfo adInfo) { }
        void RewardedOnAdInfoChangedEvent(LevelPlayAdInfo adInfo) { }
    }

    public void ShowRewarded()
    {
        if (!RewardedAd.IsAdReady())
        {
            RewardedAd.LoadAd();
        }

        if (RewardedAd.IsAdReady())
        {
            RewardedAd.ShowAd();
        }
    }

    private void RetrieveAdsCooldown(OnRemoteConfigValuesFetched evt)
    {
        _adsCooldown = evt.appConfig.GetInt("AdsCooldown", _adsCooldown);
    }
}
