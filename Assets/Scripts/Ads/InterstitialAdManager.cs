using System;
using System.Diagnostics;
using Unity.Services.LevelPlay;
using Unity.Services.RemoteConfig;

public class InterstitialAdManager
{
    private LevelPlayInterstitialAd _interstitialAd;
    public InterstitialAdManager()
    {
        if (_interstitialAd == null)
        {
            _interstitialAd = new LevelPlayInterstitialAd("ag5z1prg1ezctjf4");
            PrepareEvents();
        }

        PrepareAd();
    }

    private void PrepareAd()
    {
        if (!_interstitialAd.IsAdReady())
        {
            _interstitialAd.LoadAd();
        }
    }

    public void ShowAd()
    {
        if (_interstitialAd.IsAdReady())
        {
            _interstitialAd.ShowAd();
        }
        
        PrepareAd();
    }

    private void PrepareEvents()
    {
        _interstitialAd.OnAdLoaded += InterstitialOnAdLoadedEvent;
        _interstitialAd.OnAdLoadFailed += InterstitialOnAdLoadFailedEvent;
        _interstitialAd.OnAdDisplayed += InterstitialOnAdDisplayedEvent;
        _interstitialAd.OnAdDisplayFailed += InterstitialOnAdDisplayFailedEvent;
        _interstitialAd.OnAdClicked += InterstitialOnAdClickedEvent;
        _interstitialAd.OnAdClosed += InterstitialOnAdClosedEvent;
        _interstitialAd.OnAdInfoChanged += InterstitialOnAdInfoChangedEvent;
    }
    

    void InterstitialOnAdLoadedEvent(LevelPlayAdInfo adInfo) { }
    void InterstitialOnAdLoadFailedEvent(LevelPlayAdError error) { PrepareAd(); }
    void InterstitialOnAdDisplayedEvent(LevelPlayAdInfo adInfo) { PrepareAd(); }
    void InterstitialOnAdDisplayFailedEvent(LevelPlayAdInfo adInfo, LevelPlayAdError error) { }
    void InterstitialOnAdClickedEvent(LevelPlayAdInfo adInfo) { }
    void InterstitialOnAdClosedEvent(LevelPlayAdInfo adInfo) { PrepareAd(); }
    void InterstitialOnAdInfoChangedEvent(LevelPlayAdInfo adInfo) { }
}
