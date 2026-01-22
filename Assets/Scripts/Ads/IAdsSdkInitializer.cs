using System;
using System.Threading;
using System.Threading.Tasks;

public interface IAdsSdkInitializer : IDisposable
{
    Task<InitResult> InitializeAsync(CancellationToken ct = default);
}
