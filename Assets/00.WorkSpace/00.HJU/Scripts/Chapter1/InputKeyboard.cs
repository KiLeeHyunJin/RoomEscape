using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class InputKeyboard : MonoBehaviour
{
    [SerializeField] int ChoiceCost;

    [Header("Answer")]
    [SerializeField] string Answer;
    [SerializeField] string Submission;

    [SerializeField] TMP_Text[] text;
    [SerializeField] GameObject errorMsg;

    [SerializeField] bool changeQuestion;
    [SerializeField] int questionNum;
    public void InputSubmission(string some)
    {
        if (ChoiceCost > 0)
        {
            ChoiceCost--;
            StringPlus(some);
        }
        else
            return;
    }

    public void StringPlus(string some)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(some);
        Submission = Submission + sb.ToString();

        for (int i = 0; i < Submission.Length; i++)
        {
            if (i < text.Length)
            {
                text[i].text = Submission[i].ToString();
            }
        }
    }

    public void CheckAnswer()
    {
        if (Answer == Submission)
        {
            Debug.Log("Success");
            Manager.Chapter._clickObject.state++;
            Manager.Chapter.HintData.SetClearQuestion(questionNum);
            Manager.UI.ClosePopUpUI();
            Manager.Sound.PlayButtonSound(10);
        }

        else
        {
            StartCoroutine(errorMsgcall());
            Debug.Log("Fail");
        }
    }

    public void DeleteLastCharacter()
    {
        if (Submission.Length > 0)
        {
            Submission = Submission.Substring(0, Submission.Length - 1);
            ChoiceCost++;

            // Update the TMP_Text elements
            for (int i = 0; i < text.Length; i++)
            {
                if (i < Submission.Length)
                {
                    text[i].text = Submission[i].ToString();
                }
                else
                {
                    text[i].text = string.Empty;
                }
            }
        }
    }
    private IEnumerator errorMsgcall()
    {
        errorMsg.SetActive(true);
        yield return new WaitForSeconds(1);
        errorMsg.SetActive(false);
    }
}
