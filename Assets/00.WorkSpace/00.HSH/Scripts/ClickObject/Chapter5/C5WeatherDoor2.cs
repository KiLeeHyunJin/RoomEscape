using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C5WeatherDoor2 : ClickObject
{
    [SerializeField] PopUpUI secondPopUp;
    public override void InteractEffect()
    {
        state = 1;
        Manager.Chapter.HintData.SetClearQuestion(4);
        Manager.Sound.PlayButtonSound(24);
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
                }
                break;
            case 1:
                Manager.UI.ShowPopUpUI(secondPopUp);
                break;
        }
    }
}
