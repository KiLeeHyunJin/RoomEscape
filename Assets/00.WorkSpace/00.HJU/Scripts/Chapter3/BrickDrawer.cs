using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickDrawer : ClickObject
{
    [SerializeField] GameObject drawer1;
    [SerializeField] GameObject drawer2;
    [SerializeField] bool initialized = true;
    [SerializeField] bool check = true;

    private void Update()
    {
        if (initialized)
        {
            initialized = false;
            drawer2.SetActive(false);
        }

        if (state == 1 && check)
        {
            check = false;
            drawer1.SetActive(false);
            drawer2.SetActive(true);
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
