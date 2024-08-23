using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintUIViewer : PopUpUI
{
    protected override void Start()
    {
        base.Start();
        Manager.Text.TextChange();
    }
}
