using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : ClickObject
{
    [SerializeField] PopUpUI RicePocketGet;
    public override void ClickEffect()
    {
        switch(state)
        {
            case 0:
                PopUp(); 
                break;
            case 1:
                Interact();
                break;
            case 2:
                break;
        }
    }
    public override void InteractEffect()
    {
        GetItem(item);
        Manager.UI.ShowPopUpUI(RicePocketGet);
        state = 2;
    }
}
