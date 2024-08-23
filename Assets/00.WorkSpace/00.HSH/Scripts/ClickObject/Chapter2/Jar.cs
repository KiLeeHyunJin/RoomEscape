using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jar : ClickObject
{
    private void Start()
    {
        changeActiveValue = 1;
    }
    public override void ClickEffect()
    {
        switch(state)
        {
            case 0:
                GetItem(item);
                PopUp();
                state = 1;
                break;
            case 1:
                break;
        }
    }
}