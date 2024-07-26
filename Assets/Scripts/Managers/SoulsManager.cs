using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulsManager : MonoBehaviour
{
    public static SoulsManager instance; // Singleton instance
    public int soulCount;
    public event Action OnSoulsChanged;
    private CharacterUnlocker characterUnlocker;

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
        // Find the CharacterUnlocker instance
        characterUnlocker = FindObjectOfType<CharacterUnlocker>();

        LoadSouls();
    }

    public void AddSouls(int amount)
    {
        soulCount += amount;
        // Save the updated soul count
        PlayerPrefs.SetInt("soulCount", soulCount);
        OnSoulsChanged?.Invoke();
    }

    public bool SpendSouls(int amount)
    {
        if (soulCount >= amount)
        {
            soulCount -= amount;
            PlayerPrefs.SetInt("SoulCount", soulCount);
            OnSoulsChanged?.Invoke();
            return true;
        }
        else
        {
            return false;
        }
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
        
        // Reset the character unlock status
        if (characterUnlocker != null)
        {
            characterUnlocker.ResetCharacter();
        }
    }
}

