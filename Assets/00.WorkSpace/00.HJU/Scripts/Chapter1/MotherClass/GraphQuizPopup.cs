using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GraphQuizPopup : PopUpUI
{
    [SerializeField] GameObject clearMessage;
    [SerializeField] private int[] dials = new int[5];
    [SerializeField] int maxState;
    [SerializeField] int[] correctAnswer;
    public DialGameButton[] CookieButton;
    [SerializeField] Sprite[] changeSprites;

    [Header("EndPhase")]
    [SerializeField] int changeState;
    [SerializeField] bool getitem;
    [SerializeField] bool changeQuestion;
    [SerializeField] int questionNum;
    public void DialClick(int i)
    {
        if (i < 1 || i > dials.Length)
        {
            Debug.LogWarning("Invalid dial index");
            return;
        }

        dials[i - 1] += 1;
        CookieButton[i - 1].state += 1;
        if (dials[i - 1] >= maxState)
        {
            dials[i - 1] = 0;
            CookieButton[i - 1].state = 0;
        }
        CheckSprite();
        CheckAnswer();
    }

    public void CheckAnswer()
    {
        for (int j = 0; j < dials.Length; j++)
        {
            if (dials[j] != correctAnswer[j])
            {
                return;
            }
        }
        GetItemActive();
        if ( clearMessage != null)
        {
            clearMessage.SetActive(true);
        }
        Manager.Chapter._clickObject.state = changeState;
        Manager.Chapter._clickObject?.ChangeImage();
        if( changeQuestion == true)
        {
            Manager.Chapter.HintData.SetClearQuestion(questionNum);
        }
        GameEffect();
    }

    private void CheckSprite()
    {
        for (int i = 0; i < CookieButton.Length; i++)
        {
            switch (CookieButton[i].state)
            {
                case 0:
                    //DialGameButton button = dialButtons[i];
                    //GameObject gameObject = button.gameObject;
                    //Image img = gameObject.GetComponent<Image>();

                    //img.sprite = changeSprites[0];
                    CookieButton[i].GetComponent<Image>().sprite = changeSprites[0];
                    break;
                case 1:
                    CookieButton[i].GetComponent<Image>().sprite = changeSprites[1];
                    break;
                case 2:
                    CookieButton[i].GetComponent<Image>().sprite = changeSprites[2];
                    break;
                case 3:
                    CookieButton[i].GetComponent<Image>().sprite = changeSprites[3];
                    break;
            }
        }
    }
    public void GetItemActive()
    {
        if(getitem)
        {
            Manager.Chapter._clickObject.GetItem(Manager.Chapter._clickObject.item);
        }
    }
    public virtual void GameEffect()
    {
        // 아이템, 상태, 클리어 메세지 활성화 등은 위에서 지정해놨으나 혹시 추가 효과 필요하면 이 클래스 상속하고 이 함수에 작성
    }
}
