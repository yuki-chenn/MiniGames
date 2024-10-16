using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{

    public GameObject goPrefab;

    public int poolSize = 50;

    public Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        if(goPrefab == null)
        {
            Debug.LogError("goPrefab is null");
            return;
        }

        for (int i = 0; i < poolSize; i++)
        {
            GameObject go = Instantiate(goPrefab, transform);
            go.SetActive(false);
            pool.Enqueue(go);
        }
    }

    public GameObject GetObject()
    {
        GameObject ret = null;
        if (pool.Count == 0)
        {
            ret = Instantiate(goPrefab, transform);
            poolSize++;
        }
        else
        {
            ret = pool.Dequeue();
        }
        ret.SetActive(true);
        return ret;
    }

    public void ReturnObject(GameObject go)
    {
        go.SetActive(false);
        go.transform.parent = transform;
        pool.Enqueue(go);
    }
}
