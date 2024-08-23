using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carpet : ClickObject
{
    [SerializeField] PopUpUI carpetGetPopUp;
    public override void ClickEffect()
    {
        switch(state)
        {
            case 0:
                PopUp();
                break;
            case 1:
                Manager.UI.ShowPopUpUI(carpetGetPopUp);
                GetItem(item);
                state = 2;
                gameObject.SetActive(false);
                break;
            case 2:
                break;
        }
    }
}
