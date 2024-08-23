using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WrongGameGroup : MonoBehaviour
{
    [Header("ClearCheck")]
    [SerializeField] List<bool> WrongGameClearState;
    [SerializeField] DataManager.GameDataName groupName;

    [Header("ByChapter")]
    [SerializeField] List<Image> images;
    [SerializeField] List<Button> buttons;

    [Header("ForChange")]
    [SerializeField] Sprite[] myImages;

    [Header("ByWrongGame")]
    [SerializeField] TextMeshProUGUI[] texts;
    [SerializeField] string successMsgNum;
    [SerializeField] string failMsgNum;

    [Header("PopUpUI")]
    [SerializeField] PopUpUI[] wrongGames;

    
    private void Start()
    {
        
    }
    private void OnEnable()
    {
        //openState 할당 및 초기화
        WrongGameStateSetting();
        //AfterGameClear();
    }
    void WrongGameStateSetting()
    {
        WrongGameListInit();
    }
    void WrongGameListInit()
    {
        if (WrongGameClearState == null) //리스트가 선언되어있지않다면 카파시티 설정과 선언
            WrongGameClearState = new((int)buttons.Count);
        else //존재한다면 리스트 초기화 후 카파시티 설정
        {
            WrongGameClearState.Clear();
            if (WrongGameClearState.Capacity != (int)buttons.Count)
                WrongGameClearState.Capacity = (int)buttons.Count;
        }
        BasicSetting();
    }
    void BasicSetting()
    {
        WrongGameClearState = new List<bool>(buttons.Count);
        WrongGameClearState = new (Manager.Data.GetClearAlbumState(groupName));
        SetByGameClear();
    }
    void SetByGameClear()
    {
        for (int i = 0; i < texts.Length; i++)
        {
            Debug.Log($"WrongGameClearState[i] : {WrongGameClearState[i]}");
            if (WrongGameClearState[i] == true)
            {
                Debug.Log($"WrongGameClearState[i] : {WrongGameClearState[i]}");
                texts[i].text = "Clear!!!";
                Manager.Text.TextChange();
            }
            else
            {
                texts[i].text = "Chellenge!!!";
                Manager.Text.TextChange();
            }
        }
    }

    void AfterGameClear()
    {
        for(int i = 0; i < buttons.Count; i++)
        {
            WrongGameClearState[i] = Manager.Data.GetClearAlbumState(groupName,i);
            Debug.Log($"{i} : {Manager.Data.GetClearAlbumState(groupName, i)}");
        }
        SetByGameClear();
    }
    public void ChapterClearMode()
    {
        for (int i = 0; i < images.Count; i++)
        {
            images[i].sprite = myImages[0];
            buttons[i].enabled = true;
        }
    }
    public void ChapterUnClearMode()
    {
        for (int i = 0; i < images.Count; i++)
        {
            images[i].sprite = myImages[1];
            buttons[i].enabled = false;
        }
    }
    public void CallWrongGame(int i)
    {
        Manager.UI.ShowPopUpUI(wrongGames[i]);
    }
}