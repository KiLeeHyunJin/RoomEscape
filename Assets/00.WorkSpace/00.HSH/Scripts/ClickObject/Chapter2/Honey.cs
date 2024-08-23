using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Honey : ClickObject
{
    public override void ClickEffect()
    {
        switch(state)
        {
            case 0:
                Interact();
                break;
            case 1:
                break;
        }
    }
    public override void InteractEffect()
    {
        state = 1;
        GetItem(item);
        gameObject.SetActive(false);
        ChangeImage();
        PopUp();
        Debug.Log("상호작용 됨");
    }
}
