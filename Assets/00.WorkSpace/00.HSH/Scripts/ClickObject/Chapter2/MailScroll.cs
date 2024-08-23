using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MailScroll : ClickObject
{
    private void Start()
    {
        changeActiveValue = 1;
    }
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
        Debug.Log("상호작용 됨");
        GetItem(Key);
        GetItem(item);
        PopUp();
        Manager.Chapter.HintData.SetClearQuestion(2);
        state = 1;
    }
}
