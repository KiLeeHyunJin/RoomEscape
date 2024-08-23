using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LockQuiz : PopUpUI
{
    [SerializeField] GameObject clearMessage;
    [SerializeField] ScriptableItem puzzleReward;
    [SerializeField] int changeState;

    public Button[] buttons;
    public TextMeshProUGUI[] buttonTexts;

    private string[] texts = { "I", "L", "E", "F" };
    private int[] clickCounts;

    protected override void Start()
    {
        clickCounts = new int[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnButtonClick(index));
        }
    }

    void OnButtonClick(int buttonIndex)
    {
        clickCounts[buttonIndex] = (clickCounts[buttonIndex] + 1) % texts.Length;
        buttonTexts[buttonIndex].text = texts[clickCounts[buttonIndex]];

        CheckAnswer();
    }

    void CheckAnswer()
    {
        //Debug.Log("정답 체크");
        if (buttonTexts[0].text == "L" &&
            buttonTexts[1].text == "I" &&
            buttonTexts[2].text == "F" &&
            buttonTexts[3].text == "E")
        {
            if (clearMessage != null)
            {
                clearMessage.SetActive(true);
            }

            if (Manager.Chapter._clickObject != null)
            {
                if (Manager.Chapter._clickObject.item != null)
                {
                    Manager.Chapter._clickObject.GetItem(Manager.Chapter._clickObject.item);
                }
            }

            //Debug.Log("정답 확인");
        }

        Close();
    }

    public void CloseThis()
    {
        Close();
    }
}
