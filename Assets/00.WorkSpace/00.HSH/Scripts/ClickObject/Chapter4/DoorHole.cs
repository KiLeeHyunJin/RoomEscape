using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHole : ClickObject
{
    public override void InteractEffect()
    {
        Manager.Game.GameClearPopup();
    }
    public override void ClickEffect()
    {
        switch(state)
        {
            case 0:
                Interact();
                break;
        }
    }
}
