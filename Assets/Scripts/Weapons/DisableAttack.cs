using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAttack : MonoBehaviour
{
    float timeToDisable = 0.5f; // How long the attack will last
    float timer;

    private void OnEnable()
    {
        timer = timeToDisable;
    }

    private void LateUpdate()
    {
        timer -= Time.deltaTime;
        if (timer < 0f)
        {
            gameObject.SetActive(false); // Removes attack
        }
    }
}
