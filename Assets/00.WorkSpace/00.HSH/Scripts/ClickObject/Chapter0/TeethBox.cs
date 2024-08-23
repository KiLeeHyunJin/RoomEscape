using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeethBox : ClickObject
{
    public override void ClickEffect()
    {
        switch(state)
        {
            case 0:
                Manager.Text.TextChange();
                PopUp();
                gameObject.SetActive(false);
                GetItem(item);
                break;
        }
    }
}
