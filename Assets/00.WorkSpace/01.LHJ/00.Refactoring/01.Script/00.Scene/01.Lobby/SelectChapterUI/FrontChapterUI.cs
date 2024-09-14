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

    public void Init(Action<int> selectBtnEventMethod, int textId, int chapterNum)
    {
        chapterSelectBtn.onClick.AddListener(() => { selectBtnEventMethod.Invoke(chapterNum); });
        chapterNameTxt.gameObject.name = textId.ToString();
    }
}
