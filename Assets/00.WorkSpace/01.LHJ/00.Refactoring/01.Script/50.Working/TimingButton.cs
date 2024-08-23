using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimingButton : PopUpUI
{
    enum Buttons
    {

    }

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
        BindButton(typeof(Buttons));

        return true;
    }

    void Judgement()
    {

    }

    IEnumerator ScaleTransRoutine()
    {
        yield return null;
    }
}
