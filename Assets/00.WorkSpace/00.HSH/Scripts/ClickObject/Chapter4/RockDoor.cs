using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockDoor : ClickObject
{
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
