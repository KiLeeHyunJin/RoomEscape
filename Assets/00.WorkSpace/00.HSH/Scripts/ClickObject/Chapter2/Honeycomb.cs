using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Honeycomb : ClickObject
{
    [SerializeField] ClickObject _Honey;
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
    public override void InteractEffect()
    {
        Manager.Chapter.HintData.SetClearQuestion(5);
        state = 1;
        ChangeImage();
        Debug.Log("상호작용 됨");
    }
    private void Update()
    {
        if(state == 0)
        {
            _Honey.gameObject.SetActive(false);
        }
        else if (state == 1)
        {
            _Honey.gameObject.SetActive(true);
        }
    }
}