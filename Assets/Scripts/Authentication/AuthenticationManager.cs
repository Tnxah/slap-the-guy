using UnityEngine;

public class AuthenticationManager : MonoBehaviour
{
    private IAuthenticator authenticator;

    private void Awake()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        authenticator = new GooglePlayGamesAuthenticator(true);
#else

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
