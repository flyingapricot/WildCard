using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderBoard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayFabManager.SendLeaderboard(15);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
