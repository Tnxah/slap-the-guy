using System.Threading;
using System.Threading.Tasks;

public class DummyAuthenticator : IAuthenticator
{
    public Task<bool> Authenticate(CancellationToken ct = default)
    {
        return Task.FromResult(true); 
    }

    public void Dispose()
    {
        
    }

    public string GetUserName()
    {
#if UNITY_EDITOR
        return "rockinmyshoe_dev";
#else
        return RandomNameGenerator.GetName();
#endif
    }
}
