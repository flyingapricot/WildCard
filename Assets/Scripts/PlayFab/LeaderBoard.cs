using MongoDB.Driver;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoard : MonoBehaviour
{
    public GameObject RowPrefab;
    public Transform rowsParent;
    public TextMeshProUGUI[] details;

    void Start()
    {
        AccountManager.Instance.GetPlayerHighScore(OnLeaderboardGet1, "WildScore");
        AccountManager.Instance.GetPlayerHighScore(OnLeaderboardGet2, "TotalKills");

    }

    void OnLeaderboardGet1(GetLeaderboardResult result)
    {
        foreach(var item in result.Leaderboard)
        {
            GameObject newGo = Instantiate(RowPrefab, rowsParent);
            details = newGo.GetComponentsInChildren<TextMeshProUGUI>();
            details[0].text = item.Position.ToString();
            details[1].text = item.DisplayName.ToString();
            details[3].text = item.StatValue.ToString();
        }
    }

    void OnLeaderboardGet2(GetLeaderboardResult result)
    {
        foreach (var item in result.Leaderboard)
        {
            details[2].text = item.StatValue.ToString();
        }
    }

 
}
