using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject objectPrefab;
    [SerializeField] private int poolSize;
    private readonly Queue<GameObject> pool = new();
    void Awake()
    {
        for (int i = 0; i < poolSize; i += 1)
        {
            GameObject obj = Instantiate(objectPrefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }
    public GameObject GetObject()
    {
        if (pool.Count == 0)
            return null;

        GameObject obj = pool.Dequeue();
        obj.SetActive(true);

        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
