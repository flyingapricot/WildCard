using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class PlayFabManager : MonoBehaviour
{

    public GameObject email;
    public GameObject password;
    public Button login;
    public Button create;
    public Button resetPass;
    public GameObject login_Text;
    public GameObject setUsername;
    public GameObject loginPage;
    public static string username = "Default003";
    // Start is called before the first frame update
    void Start()
    {
        //Login();
        setUsername.SetActive(false);
        loginPage.SetActive(true);
        create.onClick.AddListener(RegisterButton);
        login.onClick.AddListener(LoginButton);
        resetPass.onClick.AddListener(ResetPasswordButton);
    }


    private static bool IsEmailValid(string email)
    {
        string regex = @"^[^@\s]+@[^@\s]+\.(com|net|org|gov)$";

        return Regex.IsMatch(email, regex, RegexOptions.IgnoreCase);
    }

    public void ResetPasswordButton()
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = email.GetComponent<TMP_InputField>().text,
            TitleId = "5BE20"
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordReset, OnResetError);
    }

    void OnPasswordReset(SendAccountRecoveryEmailResult result)
    {
        login_Text.GetComponent<TMP_Text>().text = "Password reset mail sent!";
    }

    void OnResetError(PlayFabError error)
    {
        login_Text.GetComponent<TMP_Text>().text = "Reset failed.";
    }

    private bool DetailCheck()
    {
        if (password.GetComponent<TMP_InputField>().text.Length < 6)
        {
            login_Text.GetComponent<TMP_Text>().text = "Password must be more than 6 characters";
            return false;
        }

        if (!IsEmailValid(email.GetComponent<TMP_InputField>().text))
        {
            login_Text.GetComponent<TMP_Text>().text = "Invalid Email";
            return false;
        }

        return true;
    }

    public void LoginButton()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = email.GetComponent<TMP_InputField>().text,
            Password = password.GetComponent<TMP_InputField>().text,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams()
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginError);
    }


    public void RegisterButton()
    {
        if (DetailCheck() == false) return;

        var request = new RegisterPlayFabUserRequest
        {
            Email = email.GetComponent<TMP_InputField>().text,
            Password = password.GetComponent<TMP_InputField>().text,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFail);
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        login_Text.GetComponent<TMP_Text>().text = "Registered successfully, login now";
    }

    void OnRegisterFail(PlayFabError error)
    {
        login_Text.GetComponent<TMP_Text>().text = "Registration failed";
        Debug.Log(error.GenerateErrorReport());
    }

    void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnSuccess, OnError);
    }

    void OnSuccess(LoginResult result)
    {
        Debug.Log("Successful LOGIN!");
    }

    static void OnError(PlayFabError error)
    {
        Debug.Log("Login Failed!");
        Debug.Log(error.GenerateErrorReport());
    }

    void OnLoginSuccess(LoginResult result)
    {
        login_Text.GetComponent<TMP_Text>().text = "Correct Details entered, logging in now";
        //Always display username page for user to update username if necessary
        if(result.InfoResultPayload.PlayerProfile != null)
        {
            username = result.InfoResultPayload.PlayerProfile.DisplayName;
        }else
        {
            username = "Default002";
        }

        setUsername.SetActive(true);
        loginPage.SetActive(false);

    }
    void OnLoginError(PlayFabError error)
    {
        login_Text.GetComponent<TMP_Text>().text = "Incorrect email/password.";
        Debug.Log(error.GenerateErrorReport());
    }

    public static void SendLeaderboard(int score)
    {
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

    static void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Succsessful leaderboard sent");
    }

    public void ReturnMenu()
    {
        SceneController.instance.LoadMainMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
