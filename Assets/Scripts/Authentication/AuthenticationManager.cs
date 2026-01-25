using UnityEngine;

public class AuthenticationManager : MonoBehaviour
{
    private IAuthenticator authenticator;

    public static AuthenticationManager instance;

    private void Awake()
    {
        if (instance)
        {
            return;
        }
        else
        {
            instance = this;
        }

#if UNITY_EDITOR
        authenticator = new DummyAuthenticator();
#elif UNITY_ANDROID
        authenticator = new GooglePlayGamesAuthenticator(true);
#else
        authenticator = new DummyAuthenticator();
#endif
    }

    private async void Start()
    {
        var authRes = await authenticator.Authenticate();

        if (authRes)
        {
            print($"Authenticated as {authenticator.GetUserName()}");
        }
        else
        {
            print($"Authentication failed");
        }
    }

    public string GetUserName() => authenticator.GetUserName();
}
