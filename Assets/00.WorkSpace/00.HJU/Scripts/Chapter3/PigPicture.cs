using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigPicture : ClickObject
{
    [SerializeField] PopUpUI popupUI2;
    public override void ClickEffect()
    {
        switch (state)
        {
            case 0:
                PictureInteract();
                break;
        }
    }

    public void PictureInteract()
    {
        if (Manager.Inventory.UsingItemIndex == -1)
        {
            PopUp();
            return;
        }
        string usingItem = Manager.Inventory.GetCurrentUsingItemName();
        if (Key == null)
        {
            PopUp();
            return;
        }
        if (!string.Equals(Key.name, usingItem))
        {
            Debug.Log($"00000 {usingItem}");
            PopUp();
            return;
        }
        Manager.Inventory.OnSuccessUseItem();

        InteractEffect();
    }

    public override void InteractEffect()
    {
        Manager.Chapter.HintData.SetClearQuestion(1);
        Manager.UI.ShowPopUpUI(popupUI2);
        GetItem(item);
        Manager.Sound.PlayButtonSound(25);
    }
}
