using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PassiveItemScriptableObject", menuName = "ScriptableObjects/Passive Item")]

public class PassiveItemScriptableObject : ScriptableObject // Inheritance
{
    [SerializeField] Sprite icon; // Editor only
    public Sprite Icon { get => icon; private set => icon = value; }
    
    [SerializeField] float multiplier; // In Percentage %
    public float Multiplier { get => multiplier; private set => multiplier = value; }

    [SerializeField] int level; // Editor only
    public int Level { get => level; private set => level = value; }

    [SerializeField] GameObject nextLevelPrefab; // Prefab of the next level passive item
    public GameObject NextLevelPrefab { get => nextLevelPrefab; private set => nextLevelPrefab = value; }

}
