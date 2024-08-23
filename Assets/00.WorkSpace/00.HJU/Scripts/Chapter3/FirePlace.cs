using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePlace : ClickObject
{
    [SerializeField] GameObject firePlace1;
    [SerializeField] GameObject firePlace2;
    [SerializeField] bool initialized = true;
    [SerializeField] bool check = true;

    public override void ClickEffect()
    {
        switch (state)
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
        if (initialized)
        {
            initialized = false;
            firePlace2.SetActive(false);
        }
        if (state ==1 && check)
        {
            check = false;
            firePlace1.SetActive(false);
            firePlace2.SetActive(true);
        }

    }

    public override void InteractEffect()
    {
        state = 1;
        Manager.Chapter.HintData.SetClearQuestion(7);
        Manager.Sound.PlayButtonSound(30);
    }
}
