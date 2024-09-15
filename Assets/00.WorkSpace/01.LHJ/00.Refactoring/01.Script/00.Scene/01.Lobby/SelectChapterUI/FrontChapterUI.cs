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
    public void Init(Action<int> selectBtnEventMethod, int textId, int chapterNum, Sprite _icon)
    {
        chapterSelectBtn.onClick.AddListener(() => { selectBtnEventMethod.Invoke(chapterNum); });
        chapterNameTxt.gameObject.name = textId.ToString();
        icon.sprite = _icon;
        UpdateDownloadState();
    }

    public void UpdateDownloadState()
    {

    }
}
