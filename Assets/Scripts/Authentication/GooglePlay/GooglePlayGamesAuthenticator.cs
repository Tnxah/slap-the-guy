#if UNITY_ANDROID
using GooglePlayGames.BasicApi;
using GooglePlayGames;
#endif
using PlayFab.ClientModels;
using PlayFab;
using System.Threading.Tasks;
using UnityEngine;
using System;
using System.Threading;

public class GooglePlayGamesAuthenticator 
    #if UNITY_ANDROID 
    : IAuthenticator 
    #endif
{
#if UNITY_ANDROID
    private Task<bool> _authTask;
    private TaskCompletionSource<bool> _tcs;

    private readonly bool loginToPlayfab;

    public GooglePlayGamesAuthenticator(bool loginToPlayfab = false)
    {
        this.loginToPlayfab = loginToPlayfab;
    }

    public string GetUserName() => PlayGamesPlatform.Instance.IsAuthenticated() ? PlayGamesPlatform.Instance.GetUserDisplayName() : RandomNameGenerator.GetName();

    public Task<bool> Authenticate(CancellationToken ct = default)
    {
        if (PlayGamesPlatform.Instance.IsAuthenticated()) return Task.FromResult(true);
        if (_authTask != null) return _authTask;

        _tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        _authTask = _tcs.Task;

        if (ct.CanBeCanceled)
            ct.Register(() => _tcs.TrySetResult(false));

        try
        {
            PlayGamesPlatform.Activate()?.Authenticate(ProcessAuthentication);
        }
        catch (Exception e)
        {
            _tcs.TrySetResult(false);
        }

        return _authTask;
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            if(loginToPlayfab)
                PlayGamesPlatform.Instance.RequestServerSideAccess(false, ProcessServerAuthCode);
        }
        else
        {
            //PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);

            // Disable your integration with Play Games Services or show a login button
            // to ask users to sign-in. Clicking it should call
            // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
        }
    }

    private void ProcessServerAuthCode(string serverAuthCode)
    {
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

    public void Dispose()
    {
        _tcs.TrySetResult(false);
        _tcs = null;
        _authTask = null;
    }
#endif
}
