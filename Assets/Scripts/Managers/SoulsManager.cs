using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulsManager : MonoBehaviour
{
    public static SoulsManager instance { get; private set; } // Singleton instance
    public int soulCount;

    void Awake()
    {
        // Singleton pattern implementation
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Load the saved soul count
        soulCount = PlayerPrefs.GetInt("soulCount", 0);
    }

    public void AddSouls(int amount)
    {
        soulCount += amount;
        // Save the updated soul count
        PlayerPrefs.SetInt("soulCount", soulCount);
    }

    public int GetSoulCount()
    {
        return soulCount;
    }

    // Optional: Use this method to reset souls if needed
    public void ResetSouls()
    {
        soulCount = 0;
        PlayerPrefs.SetInt("soulCount", soulCount);
    }
}

