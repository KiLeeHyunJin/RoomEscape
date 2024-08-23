using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handkerchief : ClickObject
{
    public override void ClickEffect()
    {
        GetItem(item);
        PopUp();
        gameObject.SetActive(false);
    }
}
