using System;
using UnityEngine;

public class Chapter0KeyBoard : InputMiniChange
{
    [SerializeField] GameObject clearMessage; 

    public override void ClearEffect()
    {
        clearMessage.SetActive(true);
        Manager.Chapter._clickObject.state = 1;
        Manager.Chapter._clickObject.GetItem(Manager.Chapter._clickObject.item);
        //Manager.Chapter.QuestionDataBases[0].Questions[0].cleared = true;
        Manager.Text.TextChange();
    }
}
