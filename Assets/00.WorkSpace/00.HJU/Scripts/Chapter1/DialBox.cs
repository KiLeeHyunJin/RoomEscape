using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialBox : ClickObject
{
    private void Start()
    {
        changeImageValue = 1;
    }
    public override void ClickEffect()
    {
        switch(state)
        {
            case 0:
                PopUp();
                break;
            case 1:
                break;
        }
    }
}
