using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;
    public GameObject damageTextPrefab;
    private readonly List<GameObject> pooledObjects = new();

    void Awake()
    {
        instance = this;
    }

    public GameObject GetPooledObject()
    {
        foreach (var obj in pooledObjects)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        GameObject newObj = Instantiate(damageTextPrefab);
        pooledObjects.Add(newObj);
        return newObj;
    }
}
