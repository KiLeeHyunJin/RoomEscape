using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C5Paper : ClickObject
{
    public override void ClickEffect()
    {
        switch(state)
        {
            case 0:
                PopUp();
                break;
        }
    }
}
