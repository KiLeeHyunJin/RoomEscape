using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter4Rock : ClickObject
{
    [SerializeField] ClickObject lamp;
    public override void ClickEffect()
    {
        switch(state)
        {
            case 0:
                Interact();
                if (state == 0)
                {
                    Manager.Sound.PlayButtonSound(0);
                }
                break;
            case 1:
                break;
        }
    }
    public override void InteractEffect()
    {
        lamp.state = 1;
        Manager.Chapter.HintData.SetClearQuestion(1);
        gameObject.SetActive(false);
        Manager.Sound.PlayButtonSound(36);
    }
}