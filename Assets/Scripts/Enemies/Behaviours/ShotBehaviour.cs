using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotBehaviour : MonoBehaviour
{
    public float speed = 20f;
    public Rigidbody2D rb;
    public int damage = 1;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.CompareTag("Player"))
        {
            hitInfo.GetComponent<PlayerStats>().CurrentHealth -= damage;
            Debug.Log("PLAYER!");
            Destroy(gameObject);
        }


    }

    // Update is called once per frame
    void Update()
    {

    }
}
