using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropRandomizer : MonoBehaviour
{
    public List<GameObject> propSpawnPoints; // Prop Locations
    public List<GameObject> propPrefabs; 

    void Start()
    {
        SpawnProps();
    }

    void SpawnProps()
    {
        foreach (GameObject sp in propSpawnPoints)
        {
            int rng = Random.Range(0, propPrefabs.Count); // Randomize which prop to spawn
            GameObject prop = Instantiate(propPrefabs[rng], sp.transform.position, Quaternion.identity);
            prop.transform.parent  = sp.transform; // Organize props into their respective location GameObject
        }
    }
}
