using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlbumOrGame : PopUpUI
{
    [SerializeField] PopUpUI albumUI;
    [SerializeField] PopUpUI wrongGameUI;
    
    public void SummonAlbum()
    {
        Manager.UI.ShowPopUpUI(albumUI);
    }
    public void SummonWrongArt() 
    {
        Manager.UI.ShowPopUpUI(wrongGameUI);
    }
}
