using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{
    public static PlayerInfo instance;
    public static string email;
    public static string username;
    // Start is called before the first frame update

    public static PlayerInfo getInstance()
    {
        if(instance == null)
        {
            instance = new PlayerInfo();
        }
        return instance;
    }

    public void setEmail(string new_email)
    {
        email = new_email;
    }

    public void setUsername(string new_username)
    {
        username = new_username;
    }

    public string getUsername()
    {
        return username;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
