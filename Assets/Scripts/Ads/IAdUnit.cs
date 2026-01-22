using System;
using System.Threading;
using System.Threading.Tasks;

public interface IAdUnit : IDisposable
{
    public abstract Task<bool> LoadAsync(CancellationToken ct = default);
    public abstract bool IsReady { get; }
}
