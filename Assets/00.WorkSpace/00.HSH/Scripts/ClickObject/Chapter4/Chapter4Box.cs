using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter4Box : ClickObject
{
    public override void ClickEffect()
    {
        switch(state)
        {
            case 0:
                PopUp();
                break;
            case 1:
                break;
        }
    }
    private void Update()
    {
        if ( state == 1)
        {
            ButtonImage.sprite = changeSprite;
        }
    }
}
