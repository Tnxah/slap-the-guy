using System;
using System.Threading;
using System.Threading.Tasks;

public interface IAuthenticator : IDisposable
{
    public abstract Task<bool> Authenticate(CancellationToken ct = default);

    public abstract string GetUserName();
}
