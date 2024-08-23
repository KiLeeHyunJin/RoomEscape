using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinePointPopup : PopUpUI
{
    [SerializeField] GameObject clearMessage;
    [SerializeField] private int[] dials;
    [SerializeField] int[] correctAnswer;
    public DialGameButton[] dialButton;
    [SerializeField] bool changeQuestionClear;
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
        if (dials[i - 1] >= 5)
        {
            dials[i - 1] = 0;
            dialButton[i - 1].state = 0;
        }

        CheckHeight();
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
        if(changeQuestionClear == true)
        {
            Manager.Chapter.HintData.SetClearQuestion(questionNum);
        }
        Manager.Chapter._clickObject.state = 1;
        clearMessage.SetActive(true);
    }

    public void CheckHeight()
    {
        for (int i = 0; i < dialButton.Length; i++)
        {
            float positionY = -200f +(100f * dialButton[i].state);
            dialButton[i].transform.localPosition = new Vector3(0, positionY, 0);

            if (dialButton[i].transform.localPosition.y >= 600f)
            {
                dialButton[i].transform.localPosition = new Vector3(0, -200f, 0);
            }
        }
    }
}
