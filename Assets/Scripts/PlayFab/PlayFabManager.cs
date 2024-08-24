using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System;
using Unity.VisualScripting;
using static UnityEngine.EventSystems.EventTrigger;

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
    public GameObject Username;
    public GameObject UserLogin;
    public GameObject InfoText;
    public Button ReturnToMenu;
    public Button LogOut;
    public Button ChangePassword;
    // Start is called before the first frame update

    public void UpdateUsername(GetAccountInfoResult result)
    {
        if (result != null && result.AccountInfo != null)
        {
            // Access the email address
            if (result.AccountInfo.PrivateInfo != null)
            {
                Username.GetComponent<TMP_Text>().text = result.AccountInfo.PrivateInfo.Email;
                Username.GetComponent<TMP_Text>().color = Color.green;
            }
            else
            {
                Debug.Log("No email address found for this player.");
            }
        }
        else
        {
            Debug.Log("Account info is null.");
        }
    }

    public void Logout()
    {
        AccountManager.Instance.LogOut();
        login_Text.GetComponent<TMP_Text>().text = "Login to continue";
        email.GetComponent<TMP_InputField>().text = "";
        password.GetComponent<TMP_InputField>().text = "";
        loginPage.SetActive(true);
        UserLogin.SetActive(false);
    }

    public void ResetPassword()
    {
        AccountManager.Instance.ResetPassword(Username.GetComponent<TMP_Text>().text, OnPasswordReset, OnResetError);
        InfoText.GetComponent<TMP_Text>().text = "Reset password, log out and re-login.";
    }

    void Start()
    {
        //Login();
        if(AccountManager.Instance.isPlayerLoggedIn() == true) 
        {
            loginPage.SetActive(false);
            UserLogin.SetActive(true);
            AccountManager.Instance.GetAccountInfo(UpdateUsername);
            ReturnToMenu.onClick.AddListener(ReturnMenu);
            LogOut.onClick.AddListener(Logout);
            ChangePassword.onClick.AddListener(ResetPassword);
            return;
        }
        setUsername.SetActive(false);
        loginPage.SetActive(true);
        create.onClick.AddListener(RegisterButton);
        login.onClick.AddListener(LoginButton);
        resetPass.onClick.AddListener(ResetPasswordButton);
        UserLogin.SetActive(false);
        ReturnToMenu.onClick.AddListener(ReturnMenu);
        LogOut.onClick.AddListener(Logout);
        ChangePassword.onClick.AddListener(ResetPassword);
    }


    private static bool IsEmailValid(string email)
    {
        string regex = @"^[^@\s]+@[^@\s]+\.(com|net|org|gov)$";

        return Regex.IsMatch(email, regex, RegexOptions.IgnoreCase);
    }

    public void ResetPasswordButton()
    {
        AccountManager.Instance.ResetPassword(email.GetComponent<TMP_InputField>().text,OnPasswordReset,OnResetError);
        login_Text.GetComponent<TMP_Text>().text = AccountManager.Instance.getOutput();
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

    private void UpdateTotalKills(GetLeaderboardResult result)
    {
        // Assuming the player is authenticated and their PlayFab ID is known
        string playerPlayFabId = PlayFabSettings.staticPlayer.PlayFabId; // or use the player's PlayFab ID if known

        foreach (var entry in result.Leaderboard)
        {
            if (entry.PlayFabId == playerPlayFabId)
            {
                PlayerPrefs.SetInt("totalKills", entry.StatValue);
                Debug.Log($"Player Total Kills: {entry.StatValue}");
                return; // Found the player, exit early
            }
        }

    }

    private void UpdateHighScore(GetLeaderboardResult result)
    {
        // Assuming the player is authenticated and their PlayFab ID is known
        string playerPlayFabId = PlayFabSettings.staticPlayer.PlayFabId; // or use the player's PlayFab ID if known

        foreach (var entry in result.Leaderboard)
        {
            if (entry.PlayFabId == playerPlayFabId)
            {
                PlayerPrefs.SetInt("highscore", entry.StatValue);
                Debug.Log($"Player Highscore: {entry.StatValue}");
                return; // Found the player, exit early
            }
        }

    }

    public void LoginButton()
    {
        AccountManager.Instance.Login(email.GetComponent<TMP_InputField>().text, password.GetComponent<TMP_InputField>().text,OnLoginSuccess,OnLoginError);
        login_Text.GetComponent<TMP_Text>().text = AccountManager.Instance.getOutput();
    }

    void OnLoginSuccess(LoginResult result)
    {
        login_Text.GetComponent<TMP_Text>().text = "Correct Details entered, logging in now";
        AccountManager.Instance.GetAccountInfo(UpdateUsername);
        UserLogin.SetActive(true);

        //Get Player's Highscore and Highkills
        AccountManager.Instance.GetPlayerHighScore(UpdateTotalKills, "TotalKills");
        AccountManager.Instance.GetPlayerHighScore(UpdateHighScore, "WildScore");
    }

    void OnLoginError(PlayFabError error)
    {
        login_Text.GetComponent<TMP_Text>().text = "Incorrect email/password.";
    }


    public void RegisterButton()
    {
        bool detail = DetailCheck();
        if (detail)
        {
            AccountManager.Instance.Register(detail, email.GetComponent<TMP_InputField>().text, password.GetComponent<TMP_InputField>().text, OnRegisterSuccess, OnRegisterFail);
            login_Text.GetComponent<TMP_Text>().text = AccountManager.Instance.getOutput();
        }
    }
    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        login_Text.GetComponent<TMP_Text>().text = "Registered successfully, login now";
        PlayerPrefs.SetInt("totalKills", 0);
        PlayerPrefs.SetInt("highscore", 0);
    }
    void OnRegisterFail(PlayFabError error)
    {
        login_Text.GetComponent<TMP_Text>().text = "Registration failed";
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


    public void ReturnMenu()
    {
        SceneController.instance.LoadMainMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
