using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("This will be replaced by the Passive Data class.")]
[CreateAssetMenu(fileName = "PassiveItemScriptableObject", menuName = "ScriptableObjects/Passive Item")]

public class PassiveItemScriptableObject : ScriptableObject // Inheritance
{
    [SerializeField] new string name; // Name of the Passive item or its upgrade
    public string Name { get => name; private set => name = value; }
    
    [SerializeField] Sprite icon; // Editor only
    public Sprite Icon { get => icon; private set => icon = value; }
    
    [SerializeField] string description; // Description of the Passive item or its upgrade
    public string Description { get => description; private set => description = value; }
    
    [SerializeField] float multiplier; // In Percentage %
    public float Multiplier { get => multiplier; private set => multiplier = value; }

    [SerializeField] int level; // Editor only
    public int Level { get => level; private set => level = value; }

    [SerializeField] GameObject nextLevelPrefab; // Prefab of the next level passive item
    public GameObject NextLevelPrefab { get => nextLevelPrefab; private set => nextLevelPrefab = value; }

}
