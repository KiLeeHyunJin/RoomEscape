using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlbumFinale : PopUpUI
{
    [SerializeField] PopUpUI AlbumViewer;
    [SerializeField] GameObject button;
    protected override void Start()
    {
        base.Start();
        Manager.Text.TextChange();
        StartCoroutine(ButtonOn());
    }
    public void CallFinale()
    {
        Manager.Chapter.chapter = 6;
        Manager.UI.ShowPopUpUI(AlbumViewer);
    }

    IEnumerator ButtonOn()
    {
        yield return new WaitForSeconds(2f);
        button.SetActive(true);
    }
}