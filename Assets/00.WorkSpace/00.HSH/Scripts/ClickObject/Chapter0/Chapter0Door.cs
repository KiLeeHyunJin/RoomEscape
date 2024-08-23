using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter0Door : ClickObject
{
    public override void InteractEffect()
    {
        state = 1;
    }
    public override void ClickEffect()
    {
        switch(state)
        {
            case 0:
                Interact();
                Manager.Game.GameClearPopup();
                break;
            case 1:
                //PopUp();
                break;
        }
    }
}