using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnlargementViewer : PopUpUI
{
    [SerializeField] Image MyImage;
    protected override void Start()
    {
        base.Start();
        MyImage.sprite = Manager.Chapter.HintPopUpSprite;
    }
    public void ClearPopUp()
    {
        Manager.UI.ClearPopUpUI();
    }
}