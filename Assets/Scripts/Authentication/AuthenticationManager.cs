using GooglePlayGames.BasicApi;
using GooglePlayGames;
using UnityEngine;
using TMPro;
using PlayFab.ClientModels;
using PlayFab;

public class AuthenticationManager : MonoBehaviour
{
    private void Awake()
    {
#if PLATFORM_ANDROID && !UNITY_EDITOR
        PlayGamesPlatform.Activate();
        AuthenticateWithGooglePlay();
#endif
    }

    private void AuthenticateWithGooglePlay()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            print($"Name: {PlayGamesPlatform.Instance.GetUserDisplayName()}");
            PlayGamesPlatform.Instance.RequestServerSideAccess(false, ProcessServerAuthCode);
        }
        else
        {
            PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);

            // Disable your integration with Play Games Services or show a login button
            // to ask users to sign-in. Clicking it should call
            // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
        }
    }

    private void ProcessServerAuthCode(string serverAuthCode)
    {
        Debug.Log("Server Auth Code: " + serverAuthCode);

        var request = new LoginWithGooglePlayGamesServicesRequest
        {
            ServerAuthCode = serverAuthCode,
            CreateAccount = true,
            TitleId = PlayFabSettings.TitleId
        };

        PlayFabClientAPI.LoginWithGooglePlayGamesServices(request, OnLoginWithGooglePlayGamesServicesSuccess, OnLoginWithGooglePlayGamesServicesFailure);
    }

    private void OnLoginWithGooglePlayGamesServicesSuccess(LoginResult result)
    {
        Debug.Log("PF Login Success LoginWithGooglePlayGamesServices");
    }

    private void OnLoginWithGooglePlayGamesServicesFailure(PlayFabError error)
    {
        Debug.Log("PF Login Failure LoginWithGooglePlayGamesServices: " + error.GenerateErrorReport());
    }

}
