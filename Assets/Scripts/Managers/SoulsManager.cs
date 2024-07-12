using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulsManager : MonoBehaviour
{
    public static SoulsManager instance; // Singleton instance
    public int soulCount;
    public event Action OnSoulsChanged;

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
        LoadSouls();
    }

    public void AddSouls(int amount)
    {
        soulCount += amount;
        // Save the updated soul count
        PlayerPrefs.SetInt("soulCount", soulCount);
        OnSoulsChanged?.Invoke();
    }

    public void LoadSouls()
    {
        // Load the saved soul count
        soulCount = PlayerPrefs.GetInt("soulCount", 0);
        OnSoulsChanged?.Invoke();
    }

    // Optional: Use this method to reset souls if needed
    public void ResetSouls()
    {
        soulCount = 0;
        PlayerPrefs.SetInt("soulCount", soulCount);
        OnSoulsChanged?.Invoke();
    }
}

