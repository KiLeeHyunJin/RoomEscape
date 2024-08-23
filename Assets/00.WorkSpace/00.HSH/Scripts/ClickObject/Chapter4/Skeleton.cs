using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : ClickObject
{
    [SerializeField] PopUpUI bone1;
    private void Start()
    {
        changeImageValue = 1;
    }
    public override void ClickEffect()
    {
        switch(state)
        {
            case 0:
                Interact();
                if(state == 0)
                {
                    Manager.UI.ShowPopUpUI(bone1);
                    Manager.Sound.PlayButtonSound(0);
                }
                break;
            case 1:
                PopUp();
                Manager.Sound.PlayButtonSound(0);
                break;
        }
    }
    public override void InteractEffect()
    {
        state = 1;
        Manager.Chapter.HintData.SetClearQuestion(7);
        Manager.Sound.PlayButtonSound(5);
    }
}
