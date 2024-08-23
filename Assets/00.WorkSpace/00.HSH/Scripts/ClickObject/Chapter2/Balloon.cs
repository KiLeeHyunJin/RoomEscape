using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : ClickObject
{
    public override void ClickEffect()
    {
        if (Manager.Inventory.FindItem(item))
            return;
        GetItem(item);
        PopUp();
    }
}
