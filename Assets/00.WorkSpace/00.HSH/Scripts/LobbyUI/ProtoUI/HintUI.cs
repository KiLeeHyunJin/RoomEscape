using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintUI : PopUpUI
{
    [SerializeField] PopUpUI hintPopUp;
    public void UseHintYes()
    {
        //Debug.Log("Use Hint");
        Manager.UI.ClosePopUpUI();
        if(hintPopUp == null)
            hintPopUp = Manager.Resource.Load<Advertise>("Popup/Advertise");
        Manager.UI.ShowPopUpUI<PopUpUI>(Instantiate(hintPopUp));
    }
    public void UseHintNo()
    {
        //Debug.Log("Don't Use Hint");
        Manager.UI.ClosePopUpUI();
    }
}
