using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChapterFrontUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI chapterNameTxt;
    [SerializeField] Button chapterSelectBtn;
    [SerializeField] Image downloadIcon;
    int chapterNum;
    public void Init(Action<int> selectBtnEventMethod, int textId, int chapterNum, Sprite icon)
    {
        this.chapterNum = chapterNum;
        chapterSelectBtn.image.sprite = icon;
        chapterSelectBtn.onClick.AddListener(() => { selectBtnEventMethod.Invoke(this.chapterNum); });
        chapterNameTxt.gameObject.name = textId.ToString();

        UpdateDownloadIconState();
    }

    public void UpdateDownloadIconState()
    {

    }
}
