using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.LevelPlay;

public sealed class LevelPlaySdkInitializer : IAdsSdkInitializer
{
    private readonly string _appKey = "23ff76ccd";

    private bool _initialized;
    private Task<InitResult> _initTask;
    private TaskCompletionSource<InitResult> _tcs;

    public LevelPlaySdkInitializer(string appKey) => _appKey = appKey;

    public Task<InitResult> InitializeAsync(CancellationToken ct = default)
    {
        if (_initialized) return Task.FromResult(InitResult.Ok());
        if (_initTask != null) return _initTask;

        _tcs = new TaskCompletionSource<InitResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        _initTask = _tcs.Task;

        if (ct.CanBeCanceled)
        {
            ct.Register(() =>
            {
                Unsubscribe();
                _tcs.TrySetResult(InitResult.Fail("SDK initialization cancelled."));
            });
        }

        Subscribe();

        try
        {
            LevelPlay.Init(_appKey);
        }
        catch (Exception e)
        {
            Unsubscribe();
            _tcs.TrySetResult(InitResult.Fail($"LevelPlay.Init threw: {e.Message}"));
        }

        return _initTask;
    }

    private void Subscribe()
    {
        LevelPlay.OnInitSuccess += OnSuccess;
        LevelPlay.OnInitFailed += OnFail;
    }

    private void Unsubscribe()
    {
        LevelPlay.OnInitSuccess -= OnSuccess;
        LevelPlay.OnInitFailed -= OnFail;
    }

    private void OnSuccess(LevelPlayConfiguration _)
    {
        Unsubscribe();
        _initialized = true;
        _tcs.TrySetResult(InitResult.Ok());
    }

    private void OnFail(LevelPlayInitError error)
    {
        Unsubscribe();
        _tcs.TrySetResult(InitResult.Fail(error.ToString()));
    }

    public void Dispose()
    {
        Unsubscribe();
        _tcs?.TrySetResult(InitResult.Fail("Initializer disposed during initialization."));
    }
}
