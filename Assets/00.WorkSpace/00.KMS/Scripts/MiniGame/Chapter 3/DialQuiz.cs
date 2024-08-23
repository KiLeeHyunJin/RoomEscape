using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialQuiz : PopUpUI
{
    [SerializeField] GameObject clearMessage;
    [SerializeField] ScriptableItem QuizReward;
    [SerializeField] int changeState;

    public Button[] buttons;
    public TextMeshProUGUI[] buttonTexts;

    private string[] texts = { "S", "H", "I", "L", "O" };
    private int[] clickCounts;

    protected override  void Start()
    {
        clickCounts = new int [buttons.Length];

        for(int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnButtonClick(index));
        }
    }

    private void OnButtonClick(int buttonIndex)
    {
        clickCounts[buttonIndex] = (clickCounts[buttonIndex] + 1) % texts.Length;
        buttonTexts[buttonIndex].text = texts[clickCounts[buttonIndex]];

        CheckAnswer();
    }

    private void CheckAnswer()
    {
        if (buttonTexts[0].text == "S" &&
            buttonTexts[1].text == "H" &&
            buttonTexts[2].text == "I" &&
            buttonTexts[3].text == "L" &&
            buttonTexts[4].text == "O")
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

    private void CloseThis()
    {
        Close();
    }

}
