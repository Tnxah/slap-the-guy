using System;
using System.Threading;
using System.Threading.Tasks;
public class LevelPlayAdsProvider : IAdsProvider
{
    private readonly IAdsSdkInitializer _initializer;
    private readonly IRewardedAdUnit _rewarded;
    private readonly IInterstitialAdUnit _interstitial;

    private bool _initialized;

    public LevelPlayAdsProvider(string appKey, string rewardedId, string interstitialId)
    {
        _initializer = new LevelPlaySdkInitializer(appKey);
        _rewarded = new LevelPlayRewardedUnit(rewardedId);
        _interstitial = new LevelPlayInterstitialUnit(interstitialId);
    }

    public async Task<InitResult> InitializeAsync(CancellationToken ct = default)
    {
        var res = await _initializer.InitializeAsync(ct);
        _initialized = res.Success;

        if (_initialized)
        {
            _ = _rewarded.LoadAsync(ct);
            _ = _interstitial.LoadAsync(ct);
        }

        return res;
    }

    Task<bool> IAdsProvider.LoadRewardedAsync(CancellationToken ct)
    { 
        return _initialized ? _rewarded.LoadAsync(ct) : Task.FromResult(false);
    }

    public bool TryShowRewarded(Action onRewarded = null) 
    {
        return _initialized && _rewarded.TryShow(onRewarded);
    }

    Task<bool> IAdsProvider.LoadInterstitialAsync(CancellationToken ct)
    {
        return _initialized ? _interstitial.LoadAsync(ct) : Task.FromResult(false);
    }

    public bool TryShowInterstitial()
    {
        return _initialized && _interstitial.TryShow();
    }

    public void Dispose()
    {
        _initializer.Dispose();
        _rewarded.Dispose();
        _interstitial.Dispose();
    }
}