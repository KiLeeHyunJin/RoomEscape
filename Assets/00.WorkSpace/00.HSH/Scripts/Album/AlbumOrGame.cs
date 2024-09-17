using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.UI;

public class AlbumOrGame : PopUpUI
{
    private enum Buttons
    {
        AlbumBtn,
        WrongGameBtn
    }


    [SerializeField] PopUpUI albumUI;
    [SerializeField] PopUpUI wrongGameUI;

    protected override void Start()
    {
        BindButton(typeof(Buttons));

        GetButton((int)Buttons.AlbumBtn).onClick.AddListener(SummonAlbum);
        GetButton((int)Buttons.WrongGameBtn).onClick.AddListener(SummonWrongArt);
    }

    void SummonAlbum()
    {
        Manager.UI.ShowPopUpUI(albumUI);
    }
    void SummonWrongArt() 
    {
        Manager.UI.ShowPopUpUI(wrongGameUI);
    }
}
