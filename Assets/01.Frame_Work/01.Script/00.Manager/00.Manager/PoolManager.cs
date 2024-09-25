using System.Collections.Generic;
using UnityEngine;


public class PoolManager : Singleton<PoolManager>
{
    private Dictionary<int, ObjectPool> poolDic = new Dictionary<int, ObjectPool>();


    protected override void Awake()
    {
        base.Awake();
    }

    public void CreatePool(PooledObject prefab, int size, int capacity,bool dontDestoryState = false)
    {
        GameObject gameObject = new GameObject();
        gameObject.name = $"Pool_{prefab.name}";


        ObjectPool objectPool = gameObject.AddComponent<ObjectPool>();
        if (dontDestoryState)
        {
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(objectPool);
        }
        objectPool.CreatePool(prefab, size, capacity);

        poolDic.Add(prefab.GetInstanceID(), objectPool);
    }

    public void DestroyPool(PooledObject prefab)
    {
        ObjectPool objectPool = poolDic[prefab.GetInstanceID()];
        if(objectPool != null)
            Destroy(objectPool.gameObject);
        poolDic.Remove(prefab.GetInstanceID());
    }

    public void ClearPool()
    {
        foreach (ObjectPool objectPool in poolDic.Values)
        {
            Destroy(objectPool.gameObject);
        }
        poolDic.Clear();
    }

    public PooledObject GetPool(PooledObject prefab, Vector3 position, Quaternion rotation)
    {
        return poolDic[prefab.GetInstanceID()].GetPool(position, rotation);
    }
    public PooledObject GetPool(PooledObject prefab, Vector3 position = new Vector3())
    {
        return poolDic[prefab.GetInstanceID()].GetPool(position, Quaternion.identity);
    }

}
