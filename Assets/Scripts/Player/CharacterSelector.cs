using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    public static CharacterSelector instance;
    public CharacterScriptableObject characterData;
    public WeaponScriptableObject weaponData;

    void Awake()
    {
        if (instance == null) // Singleton Pattern
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            Debug.LogWarning("EXTRA " + this + " DELETED");
        }
    }

    public void DestroySingleton()
    {
        instance = null;
        Destroy(gameObject);
    }

    public static CharacterScriptableObject GetCharacterData()
    {
        return instance.characterData;
    }

    public void SelectCharacter(CharacterScriptableObject character)
    {
        characterData = character;
    }

    public static WeaponScriptableObject GetWeaponData()
    {
        return instance.weaponData;
    }

    public void SelectWeapon(WeaponScriptableObject weapon)
    {
        weaponData = weapon;
    }
}
