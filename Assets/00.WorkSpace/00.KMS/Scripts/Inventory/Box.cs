using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : InteracterController
{
    public ScriptableItem key; // 상호 작용에 필요한 아이템
    public ScriptableItem containItem; // 상호작용 성공 후 얻는 아이템

    public override void Interact()
    {
        ScriptableItem usingItem = Manager.Inventory.GetCurrentUsingItem();
        if (key != usingItem)
            return;

        Manager.Inventory.OnSuccessUseItem();
        Destroy(gameObject);
        Manager.Inventory.ObtainItem(containItem);
    }
   
}
