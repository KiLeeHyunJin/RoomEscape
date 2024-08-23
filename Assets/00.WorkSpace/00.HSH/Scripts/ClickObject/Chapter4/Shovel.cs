using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shovel : ClickObject
{
    public override void ClickEffect()
    {
        PopUp(); //만들것
        GetItem(item);
        gameObject.SetActive(false);
    }
}