using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenKey : ClickObject
{
    private void Start()
    {
        changeActiveValue = 1;
    }
    public void GetKey()
    {
        GetItem(item);
        PopUp();
        state = 1;
    }
}
