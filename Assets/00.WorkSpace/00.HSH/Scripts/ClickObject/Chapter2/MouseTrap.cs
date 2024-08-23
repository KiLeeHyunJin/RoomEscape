using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTrap : ClickObject
{
    private void Start()
    {
        //changeActiveValue = 2;
    }
    public override void ClickEffect()
    {
        switch(state)
        {
            case 0:
                break;
            case 1:
                PopUp();
                break;
        }
    }
    private void Update()
    {
        if (currentState < 2)
        {
            gameObject.SetActive(true);
        }
        else if( currentState == 2)
        {
            gameObject.SetActive(false);
        }
        return;
    }
}
