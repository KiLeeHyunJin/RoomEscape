using System.Collections;
using UnityEngine;

public abstract class BaseScene : MonoBehaviour
{
    protected virtual void Start()
    {
    }
    public abstract IEnumerator LoadingRoutine();
}
