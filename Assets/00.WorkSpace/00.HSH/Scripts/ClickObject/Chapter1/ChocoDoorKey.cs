using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChocoDoorKey : ClickObject
{
    private void Start()
    {
        changeActiveValue = 1;
    }
    public override void InteractEffect()
    {
        state = 1;
        //GetItem(containItem);
        ChangeImage();
        Debug.Log("상호작용 됨");
    }
}
