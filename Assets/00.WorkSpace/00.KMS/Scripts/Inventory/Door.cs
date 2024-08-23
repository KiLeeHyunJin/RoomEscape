using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : InteracterController
{
    public ScriptableItem target;

    public override void Interact()
    {
        ScriptableItem usingItem = Manager.Inventory.GetCurrentUsingItem();
        if (target != usingItem)
            return;

        Manager.Inventory.OnSuccessUseItem();
        Destroy(gameObject);
    }
}
