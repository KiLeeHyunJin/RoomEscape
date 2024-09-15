using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FrontChapterUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI chapterNameTxt;
    [SerializeField] Button chapterSelectBtn;
    [SerializeField] Image downloadIcon;
    [SerializeField] Image icon;
    int chapterNum;
    public void Init(Action<int> selectBtnEventMethod, int textId, int chapterNum, Sprite icon)
    {
        this.icon.sprite = icon;
        this.chapterNum = chapterNum;

        chapterSelectBtn.onClick.AddListener(() => { selectBtnEventMethod.Invoke(chapterNum); });
        chapterNameTxt.gameObject.name = textId.ToString();

        UpdateDownloadState();
    }

    public void UpdateDownloadState()
    {

    }
}
