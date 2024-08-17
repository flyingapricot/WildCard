using MongoDB.Driver;
using PlayFab.ClientModels;
using System;
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

    // List to hold references to instantiated rows
    private List<GameObject> instantiatedRows = new List<GameObject>();


    void Start()
    {
        AccountManager.Instance.GetPlayerHighScore(OnLeaderboardGet1, "WildScore");
        AccountManager.Instance.GetPlayerHighScore(OnLeaderboardGet2, "TotalKills");

    }

    void OnLeaderboardGet1(GetLeaderboardResult result)
    {

        foreach (var item in result.Leaderboard)
        {
            GameObject newGo = Instantiate(RowPrefab, rowsParent);
            details = newGo.GetComponentsInChildren<TextMeshProUGUI>();
            details[0].text = (item.Position + 1).ToString();
            details[1].text = item.DisplayName.ToString();
            details[3].text = item.StatValue.ToString();

            // Add to the list of instantiated rows
            instantiatedRows.Add(newGo);
        }

        AccountManager.Instance.GetPlayerHighScore(OnLeaderboardGet2, "TotalKills");

    }

    void OnLeaderboardGet2(GetLeaderboardResult result)
    {
        foreach (var item in result.Leaderboard)
        {
            foreach (var row in instantiatedRows)
            {
                TextMeshProUGUI[] details = row.GetComponentsInChildren<TextMeshProUGUI>();
                Debug.Log(details[1].text);
                Debug.Log(item.DisplayName.ToString());
                if (string.Equals(details[1].text, item.DisplayName.ToString()))
                {
                    details[2].text = item.StatValue.ToString();
                    break;
                }
            }
        }
    }



}
