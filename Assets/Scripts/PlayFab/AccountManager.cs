using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class AccountManager : Singleton<AccountManager>
{
    public string output;
    public bool LoggedIn = false;

    public void LogOut()
    {
        // Make the API call to PlayFab to logout
        PlayFabClientAPI.ForgetAllCredentials();
    }

    public string getOutput()
    {
        return output;
    }
    public void Login(string email,string password, Action<LoginResult> OnLoginSuccess, Action<PlayFabError> OnLoginError)
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = email,
            Password = password,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginError);
    }


    public void Register(bool detailCheck, string email, string password, Action<RegisterPlayFabUserResult> OnRegisterSuccess, Action<PlayFabError> OnRegisterFail)
    {
        if (!detailCheck) return;

        var request = new RegisterPlayFabUserRequest
        {
            Email = email,
            Password = password,
            RequireBothUsernameAndEmail = false
        };

        // Register the user
        PlayFabClientAPI.RegisterPlayFabUser(request, registerResult =>
        {
            string[] parts = email.Split('@');
            // After successful registration, update the display name
            UpdateDisplayName(parts[0], registerResult, OnRegisterSuccess, OnRegisterFail);
        }, OnRegisterFail);
    }

    public void UpdateDisplayName(string displayName, RegisterPlayFabUserResult registerResult, Action<RegisterPlayFabUserResult> OnRegisterSuccess, Action<PlayFabError> OnRegisterFail)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = displayName
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, updateResult =>
        {
            // Call the original success callback with the registration result
            OnRegisterSuccess?.Invoke(registerResult);
        }, error =>
        {
            // Call the original error callback
            OnRegisterFail?.Invoke(error);
        });
    }

    public void ResetPassword(string email,Action<SendAccountRecoveryEmailResult> OnPasswordReset,Action<PlayFabError> OnResetError)
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = email,
            TitleId = "5BE20"
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordReset, OnResetError);
    }

    public void GetAccountInfo(Action<GetAccountInfoResult> OnAccountSuccess)
    {
        var request = new GetAccountInfoRequest
        {
            // This request does not need any additional parameters
        };

        PlayFabClientAPI.GetAccountInfo(request, OnAccountSuccess, OnError);
    }


    public bool isPlayerLoggedIn()
    {
        return PlayFabClientAPI.IsClientLoggedIn();
    }

    // Call this method to retrieve the player's high score
    public void GetPlayerHighScore(Action<GetLeaderboardResult> OnGetLeaderboardSuccess,string leaderboard)
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = leaderboard,
            StartPosition = 0,
            MaxResultsCount = 100
        };

        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboardSuccess, OnError);
    }

    public void SendLeaderboardHigh()
    {
        int score = PlayerPrefs.GetInt("highscore", 0);
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "WildScore",
                    Value = score
                }
            }
        };

        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError);
        }
    }

    public void SendLeaderboardKills()
    {
        int score = PlayerPrefs.GetInt("totalKills", 0);
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "TotalKills",
                    Value = score
                }
            }
        };

        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError);
        }
    }

    void OnError(PlayFabError error)
    {
        Debug.Log("Login Failed!");
        Debug.Log(error.GenerateErrorReport());
    }

    void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Succsessful leaderboard sent");
    }



}
