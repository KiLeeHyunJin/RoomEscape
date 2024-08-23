using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicPopupUI : PopUpUI
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        GetComponentInChildren<Button>().onClick.AddListener(Manager.UI.ClosePopUpUI);
        return true;
    }

}
