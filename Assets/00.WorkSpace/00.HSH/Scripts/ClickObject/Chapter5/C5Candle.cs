using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C5Candle : ClickObject
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
