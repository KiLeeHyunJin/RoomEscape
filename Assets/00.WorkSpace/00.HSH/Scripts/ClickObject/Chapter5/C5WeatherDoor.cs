using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C5WeatherDoor : ClickObject
{
    [SerializeField] GameObject HoledDoor;
    [SerializeField] bool doorOff=true;

    public override void InteractEffect()
    {
        state = 1;
        Manager.Chapter.HintData.SetClearQuestion(2);
        HoledDoor.SetActive(true);
        gameObject.SetActive(false);
        Manager.Sound.PlayButtonSound(22);
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
    private void Update()
    {
        if (doorOff)
        {
            doorOff = false;
            HoledDoor.SetActive(false);
        }
    }
}
