using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calendar : ClickObject
{
    public override void ClickEffect()
    {
        switch(state)
        {
            case 0:
                Manager.Text.TextChange();
                PopUp();
                break;
            case 1:
                break;
        }
    }
}
