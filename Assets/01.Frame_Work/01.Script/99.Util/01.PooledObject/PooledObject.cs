using System.Collections;
using UnityEngine;

public class PooledObject : MonoBehaviour
{
    [SerializeField] bool autoRelease;
    [SerializeField] float releaseTime;

    private ObjectPool pool;
    public ObjectPool Pool { get { return pool; } set { pool = value; } }

    protected virtual void OnEnable()
    {
        if (autoRelease)
        {
            StartCoroutine(ReleaseRoutine());
        }
    }

    IEnumerator ReleaseRoutine()
    {
        yield return new WaitForSeconds(releaseTime);
        Debug.Log("Release Call");
        Release();
    }

    public void Release()
    {
        if (pool != null)
        {
            pool.ReturnPool(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
