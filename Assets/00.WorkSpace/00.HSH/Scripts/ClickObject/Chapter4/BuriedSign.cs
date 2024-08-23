using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuriedSign : ClickObject
{
    [SerializeField] PopUpUI secondPopUp;
    public override void InteractEffect()
    {
        ChangeImage();
        Manager.Chapter.HintData.SetClearQuestion(3);
        state = 1;
        Manager.Sound.PlayButtonSound(28);
    }
    public override void ClickEffect()
    {
        switch(state)
        {
            case 0:
                Interact();
                if(state == 0)
                {
                    PopUp();
                    Manager.Sound.PlayButtonSound(0);
                }
                break;
            case 1:
                Manager.Sound.PlayButtonSound(0);
                Manager.UI.ShowPopUpUI(secondPopUp);
                break;
        }
    }
}
