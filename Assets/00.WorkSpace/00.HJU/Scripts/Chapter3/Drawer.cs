using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawer : ClickObject
{
    [SerializeField] GameObject drawer;
    [SerializeField] bool initialized = true;
    [SerializeField] bool check = true;

    private void Update()
    {
        if (initialized)
        {
            initialized = false;
            drawer.SetActive(false);
        }

        if (state == 1 && check)
        {
            check = false;
            drawer.SetActive(true);
        }
    }

    public override void ClickEffect()
    {
        switch (state)
        {
            case 0:
                PopUp();
                break;

                case 1:
                break; 
        }
    }
}
