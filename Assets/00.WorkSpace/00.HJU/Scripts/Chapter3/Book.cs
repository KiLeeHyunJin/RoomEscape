using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SequentialImageMover;

public class Book : ClickObject
{
    [SerializeField] GameObject book1;
    [SerializeField] GameObject book2;
    [SerializeField] bool initialized = true;
    [SerializeField] bool check = true;

    private void Update()
    {
        if (initialized)
        {
            initialized = false;
            book2.SetActive(false);
        }
        if (state == 1 && check)
        {
            check = false;
            book1.SetActive(false);
            book2.SetActive(true);
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
