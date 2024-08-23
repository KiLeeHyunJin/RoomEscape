using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wartering : ClickObject
{
    public override void ClickEffect()
    {
        GetItem(item);
        PopUp();
        Manager.Chapter.HintData.SetClearQuestion(1);
        gameObject.SetActive(false);
    }
}
