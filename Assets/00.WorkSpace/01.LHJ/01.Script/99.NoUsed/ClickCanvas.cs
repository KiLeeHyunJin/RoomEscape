using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickCanvas : PooledObject
{
    ClickEffectRatio click;
    private void Awake()
    {
        click = GetComponentInChildren<ClickEffectRatio>();
    }
    public void SetPosition(Vector3 pos)
    {
        click.SetPosition(pos);
    }
}
