using System.Threading;
using System.Threading.Tasks;
using Unity.Services.LevelPlay;
using UnityEngine;

public class LevelPlayInterstitialUnit : IInterstitialAdUnit
{
    private readonly string _adUnitId;

    private LevelPlayInterstitialAd _ad;
    private bool _handlersAttached;

    private bool _loading;
    private Task<bool> _loadTask;
    private TaskCompletionSource<bool> _loadTcs;

    public LevelPlayInterstitialUnit(string adUnitId) => _adUnitId = adUnitId;

    public bool IsReady => _ad != null && _ad.IsAdReady();

    public Task<bool> LoadAsync(CancellationToken ct = default)
    {
        EnsureCreated();

        if (IsReady) return _loadTask;
        if (_loadTask != null) return _loadTask;

        _loadTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        _loadTask = _loadTcs.Task;

        if (ct.CanBeCanceled)
            ct.Register(() => CompleteLoad(false));

        AttachHandlersIfNeeded();
        LoadIfNeeded();

        return _loadTask;
    }

    public bool TryShow()
    {
        if (!IsReady) return false;
        _ad.ShowAd();
        return true;
    }

    private void EnsureCreated()
    {
        if (_ad != null) return;
        _ad = new LevelPlayInterstitialAd(_adUnitId);
        AttachHandlersIfNeeded();
    }

    private void AttachHandlersIfNeeded()
    {
        if (_handlersAttached || _ad == null) return;

        _ad.OnAdLoaded += OnLoaded;
        _ad.OnAdLoadFailed += OnLoadFailed;
        _ad.OnAdClosed += OnClosed;
        _ad.OnAdDisplayFailed += OnDisplayFailed;

        _handlersAttached = true;
    }

    private void DetachHandlers()
    {
        if (!_handlersAttached || _ad == null) return;

        _ad.OnAdLoaded -= OnLoaded;
        _ad.OnAdLoadFailed -= OnLoadFailed;
        _ad.OnAdClosed -= OnClosed;
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
        _ad = null;
    }
}
