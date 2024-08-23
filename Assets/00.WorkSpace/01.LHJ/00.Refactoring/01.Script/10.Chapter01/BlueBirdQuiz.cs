using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueBirdQuiz : PopUpUI
{
    enum Buttons
    {
        CloseButton,
        BlueBirdButton
    }
    enum Images
    {
        Hint1,
        Hint2,
        Hint3,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindButton(typeof(Buttons));
        BindImage(typeof(Images));

        GetButton((int)Buttons.BlueBirdButton).onClick.AddListener(Sing);
        GetButton((int)Buttons.CloseButton).onClick.AddListener(Manager.UI.ClosePopUpUI);

        GameObject[] objects = new[] {
            GetImage((int)Images.Hint1).gameObject,
            GetImage((int)Images.Hint2).gameObject,
            GetImage((int)Images.Hint3).gameObject,};

        foreach (GameObject hint in objects)
            hint.SetActive(false);

        return true;
    }
    void Sing()
    {
        GetComponent<Animator>().SetTrigger("start");
    }
}
