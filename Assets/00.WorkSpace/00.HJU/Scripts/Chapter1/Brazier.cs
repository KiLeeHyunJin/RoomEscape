using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brazier : ClickObject
{
    [SerializeField] PopUpUI popUpUI2;
    [SerializeField] PopUpUI popUpUI3;
    [SerializeField] ClickSound clickSound;

    private void Start()
    {
        changeImageValue = 1;
        clickSound = GetComponent<ClickSound>();
    }
    public override void ClickEffect()
    {
        switch (state)
        {
            case 0:
                Manager.UI.ShowPopUpUI(popUpUI);
                break;

            case 1:
                clickSound.sfxIndex = 0;
                Interact();
                if(state == 1)
                {
                    Manager.UI.ShowPopUpUI(popUpUI3);
                }
                break;

            case 2:
                Manager.UI.ShowPopUpUI(popUpUI2);
                break;

            case 3:
                break;
        }
    }


    public override void InteractEffect()
    {
        state = 2;
        Manager.UI.ShowPopUpUI(popUpUI2);
    }
}
