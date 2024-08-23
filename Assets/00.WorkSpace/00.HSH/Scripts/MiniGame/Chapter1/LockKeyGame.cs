using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LockKeyGame : PopUpUI
{
    public string[] numbers = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" }; // 표시할 문자열 배열
    
    [Header("answer")]
    [SerializeField] public string[] answers;

    [Header("text")]
    [SerializeField] public TextMeshProUGUI[] texts;

    [Header("EndPhase")]
    [SerializeField] ScriptableItem treasure;
    [SerializeField] public GameObject clearMessage;
    [SerializeField] bool questionCheck;
    [SerializeField] int questionNum;
    //public void CheckAnswer()
    //{
    //    for(int i = 0; answers.Length > i; i++)
    //    {
    //        if (answers[i] == texts[i].text)
    //        {
    //            Debug.Log("Successs");
    //        }
    //        else
    //        {
    //            Debug.Log("Fail");
    //            return;
    //        }
    //    }
    //}

    public void CheckAnswer()
    {
        for (int i = 0; i < answers.Length; i++)
        {
            if (answers[i] != texts[i].text)
            {
                return;
            }
        }

        if (Manager.Chapter._clickObject != null)
        {
            if (Manager.Chapter._clickObject.item != null)
            {
                Manager.Chapter._clickObject.GetItem(Manager.Chapter._clickObject.item);
            }
        }
        if (treasure != null)
        {
            Manager.Inventory.ObtainItem(treasure);
        }

        if (clearMessage != null)
        {
            clearMessage.SetActive(true);
        }
        if(questionCheck == true)
        {
            Manager.Chapter.HintData.SetClearQuestion(questionNum);
        }
        Manager.Chapter._clickObject.state++;
    }
}