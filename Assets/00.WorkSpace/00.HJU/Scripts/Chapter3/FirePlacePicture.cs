using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePlacePicture : ClickObject
{
    [SerializeField] PopUpUI popUpUI2;
    private void Start()
    {
        changeImageValue = 1;
    }
    public override void ClickEffect()
    {
        switch (state)
        {
            case 0:
                Interact();
                if (state == 0)
                {
                    PopUp();
                }
                break;
                
                case 1:
                Manager.UI.ShowPopUpUI(popUpUI2);
                break;
        }
    }

    public override void InteractEffect()
    {
        state = 1;
        Manager.UI.ShowPopUpUI(popUpUI2);
        Manager.Chapter.HintData.SetClearQuestion(4);
        Manager.Sound.PlayButtonSound(27);
    }
}
