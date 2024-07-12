using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Souls : MonoBehaviour, ICollectible
{
    public bool Collected { get; set; } = false;
    public int soulsValue;

    public void Collect()
    {
        SoulsManager souls = FindObjectOfType<SoulsManager>();
        souls.AddSouls(soulsValue);
        Collected = true;
        Destroy(gameObject); // Destroys once collected
    }
}
