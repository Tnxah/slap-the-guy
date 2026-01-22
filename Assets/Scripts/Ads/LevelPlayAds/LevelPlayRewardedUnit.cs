using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.LevelPlay;

public class LevelPlayRewardedUnit : IRewardedAdUnit
{
    private readonly string _adUnitId;

    private LevelPlayRewardedAd _ad;
    private bool _handlersAttached;

    private bool _loading;
    private Task<bool> _loadTask;
    private TaskCompletionSource<bool> _loadTcs;

    private Action _pendingReward;

    public LevelPlayRewardedUnit(string adUnitId) => _adUnitId = adUnitId;

    public bool IsReady => _ad != null && _ad.IsAdReady();

    public Task<bool> LoadAsync(CancellationToken ct = default)
    {
        EnsureCreated();

        if (IsReady) return Task.FromResult(true);
        if (_loadTask != null) return _loadTask;

        _loadTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        _loadTask = _loadTcs.Task;

        if (ct.CanBeCanceled)
            ct.Register(() => CompleteLoad(false));

        AttachHandlersIfNeeded();
        LoadIfNeeded();

        return _loadTask;
    }

    public bool TryShow(Action onRewarded = null)
    {
        if(!IsReady) return false;
        _pendingReward = onRewarded;
        _ad.ShowAd();
        return true;
    }

    private void EnsureCreated()
    {
        if (_ad != null) return;
        _ad = new LevelPlayRewardedAd(_adUnitId);
        AttachHandlersIfNeeded();
    }

    private void AttachHandlersIfNeeded()
    {
        if (_handlersAttached || _ad == null) return;

        _ad.OnAdLoaded += OnLoaded;
        _ad.OnAdLoadFailed += OnLoadFailed;
        _ad.OnAdClosed += OnClosed;
        _ad.OnAdRewarded += OnRewarded;
        _ad.OnAdDisplayFailed += OnDisplayFailed;

        _handlersAttached = true;
    }

    private void DetachHandlers()
    {
        if (!_handlersAttached || _ad == null) return;

        _ad.OnAdLoaded -= OnLoaded;
        _ad.OnAdLoadFailed -= OnLoadFailed;
        _ad.OnAdClosed -= OnClosed;
        _ad.OnAdRewarded -= OnRewarded;
        _ad.OnAdDisplayFailed -= OnDisplayFailed;

        _handlersAttached = false;
    }

    private void LoadIfNeeded()
    {
        if (_ad == null) return;
        if (_loading) return;
        if (IsReady) return;

        _loading = true;
        _ad.LoadAd();
    }

    private void OnLoaded(LevelPlayAdInfo adInfo)
    {
        _loading = false;
        CompleteLoad(true);
    }

    private void OnLoadFailed(LevelPlayAdError adError)
    {
        _loading = false;
        CompleteLoad(false);

        //TODO: retry with delay
    }

    private void OnClosed(LevelPlayAdInfo adInfo)
    {
        LoadAsync();
    }

    private void OnRewarded(LevelPlayAdInfo adInfo, LevelPlayReward reward)
    {
        var cb = _pendingReward;
        _pendingReward = null;
        cb?.Invoke();
    }

    private void OnDisplayFailed(LevelPlayAdInfo adInfo, LevelPlayAdError adError)
    {
        LoadAsync();
    }

    private void CompleteLoad(bool success)
    {
        _loadTcs?.TrySetResult(success);
        _loadTcs = null;
        _loadTask = null;
    }

    public void Dispose()
    {
        DetachHandlers();
        CompleteLoad(false);
        _pendingReward = null;
        _ad = null;
    }
}
