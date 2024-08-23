using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;


public class ClickObject : InteracterController
{
    public int currentState;
    public int changeImageValue;
    public int changeActiveValue;
    public int state 
    {
        get{ return currentState; }
        set 
        {
            currentState = value;
            switch (currentState)
            {

                default:
                    break;
            }
            if(changeImageValue != -1 && currentState >= changeImageValue)
            {
                ChangeImage();
            }
            if (currentState == changeActiveValue) 
            {
                gameObject.SetActive(!gameObject.activeSelf);
            }
            if (currentState == 1 && this is Safe)
            {
                gameObject.SetActive(false);
            }
        } 
    }
    public ScriptableItem item;
    public Image ButtonImage; // 버튼이미지 첨부용
    public Sprite changeSprite;
    [SerializeField] public int sentenceNum;
    [SerializeField] ScriptableItem key;
    public ScriptableItem Key { get { return key; } }

    [SerializeField] public PopUpUI popUpUI;
    private void Awake()
    {
        changeActiveValue = -1;
        changeImageValue = -1;
        if(Manager.Chapter.ContinueState == false)
        {
            state = 0;
        }
    }
    public void PopUp()
    {
        Manager.Chapter._clickObject = this;
        Manager.UI.ShowPopUpUI(popUpUI);
    }

    public void LogPlay(int i)
    {
        Manager.Chapter._clickObject = this;
        Debug.Log("첫 선언");
        Manager.Text.SetScene(i);
    }

    public void GetItem(ScriptableItem item)
    {
        Manager.Inventory.ObtainItem(item);

        if (!Manager.Sound.itemGetSoundMapping.TryGetValue(item.name, out int soundIndex))
        {
            soundIndex = 7; // 기본값 사운드 인덱스
        }
        Manager.Sound.PlayItemSound(soundIndex);
    }

    public void ChangeImage()
    {
        if (changeSprite != null)
        {
            ButtonImage.sprite = changeSprite;
        }
        else
        {
            return;
        }
    }
    public void Click()
    {
        Manager.Chapter._clickObject = this;
        ClickEffect();
        Manager.Text.TextChange();
    }
    public virtual void ClickEffect()
    { }

    public override void Interact()
    {
        if (Manager.Inventory.UsingItemIndex == -1)
        {
            return;
        }
        string usingItem = Manager.Inventory.GetCurrentUsingItemName();
        if (key == null)
        {
            return;
        }
        if (!string.Equals(key.name, usingItem))
        {
            Debug.Log($"00000 {usingItem}");
            return;
        }
        Manager.Inventory.OnSuccessUseItem();

        //GetItem(containItem);
        InteractEffect();
        Debug.Log("상호작용 됨");
    }
    public virtual void InteractEffect()
    { }

    public void AfterLog()
    {
        LogEffect();
        Debug.Log("여기까지 실행됨");
    }
    public virtual void LogEffect()
    {

    }

    protected virtual void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            ChapterSaveLoad saveLoad = FindObjectOfType<ChapterSaveLoad>();
            if (saveLoad == null)
                return;
            saveLoad.SaveCurrentChapter();
        }
    }
}