using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowRotation : PopUpUI
{
    [SerializeField] GameObject clearMessage;
    [SerializeField] private int[] dials = new int[5];
    [SerializeField] int[] correctAnswer;
    public DialGameButton[] dialButton;
    [SerializeField] int changeState;
    [SerializeField] bool changeQuestion; // true 면 퀴즈 클리어 실행
    [SerializeField] int questionNum;
    public void DialClick(int i)
    {
        if (i < 1 || i > dials.Length)
        {
            Debug.LogWarning("Invalid dial index");
            return;
        }

        dials[i - 1] += 1;
        dialButton[i - 1].state += 1;
        if (dials[i - 1] >= 8)
        {
            dials[i - 1] = 0;
            dialButton[i - 1].state = 0;
        }

        CheckRotation();
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

        if (Manager.Chapter._clickObject != null)
        {
            if (Manager.Chapter._clickObject.item != null)
            {
                Manager.Chapter._clickObject.GetItem(Manager.Chapter._clickObject.item);
                Debug.Log("아이템 획득");
            }
        }
        Manager.Chapter._clickObject.state = changeState;
        ChangeQuestionClear();
        clearMessage.SetActive(true);
    }

    public void CheckRotation()
    {
        for (int i = 0; i < dialButton.Length; i++)
        {
            float rotationZ = -45f * dialButton[i].state;
            dialButton[i].transform.localRotation = Quaternion.Euler(0, 0, rotationZ);
        }
    }
    private void ChangeQuestionClear()
    {
        if(changeQuestion == true)
        {
            Manager.Chapter.HintData.SetClearQuestion(questionNum);
        }
    }
}
