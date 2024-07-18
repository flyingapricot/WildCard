using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CharacterSelector : MonoBehaviour
{
    public static CharacterSelector instance;
    public CharacterData characterData;
    // public WeaponData weaponData;

    void Awake()
    {
        if (instance == null) // Singleton Pattern
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("EXTRA " + this + " DELETED");
            Destroy(gameObject);
        }
    }

    public void DestroySingleton()
    {
        instance = null;
        Destroy(gameObject);
    }

    public static CharacterData GetData()
    {
        if (instance && instance.characterData)
            return instance.characterData;
        else
        {
            // Randomly pick a character if we are playing from the editor
            #if UNITY_EDITOR
            string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
            List<CharacterData> characters = new();
            foreach (string assetPath in allAssetPaths)
            {
                if (assetPath.EndsWith(".asset"))
                {
                    CharacterData characterData = AssetDatabase.LoadAssetAtPath<CharacterData>(assetPath);
                    if (characterData != null)
                    {
                        characters.Add(characterData);
                    }
                }
            }

            // Pick a random character if we have found any characters
            if (characters.Count > 0) return characters[Random.Range(0, characters.Count)];
            #endif
        }
        return null;
    }

    public void SelectCharacter(CharacterData character)
    {
        characterData = character;
    }

    // public static WeaponData GetWeaponData()
    // {
    //     return instance.weaponData;
    // }

    // public void SelectWeapon(WeaponData weapon)
    // {
    //     weaponData = weapon;
    // }
}
