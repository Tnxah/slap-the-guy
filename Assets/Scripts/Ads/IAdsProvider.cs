using System;
using System.Threading;
using System.Threading.Tasks;

public interface IAdsProvider : IDisposable
{
    public abstract Task<InitResult> InitializeAsync(CancellationToken ct = default);

    public abstract Task<bool> LoadRewardedAsync(CancellationToken ct = default);
    public abstract bool TryShowRewarded(Action onRewarded = null);

    public abstract Task<bool> LoadInterstitialAsync(CancellationToken ct = default);
    public abstract bool TryShowInterstitial();
}

public readonly struct InitResult
{
    public bool Success { get; }
    public string Error { get; }

    private InitResult(bool success, string error)
    {
        Success = success;
        Error = error;
    }

    public static InitResult Ok() => new InitResult(true, null);
    public static InitResult Fail(string error) => new InitResult(false, error);
}
