using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialLock : PopUpUI
{
    public string[] numbers = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" }; // 표시할 문자열 배열

    [Header("answer")]
    [SerializeField] public string[] answers;

    [Header("text")]
    [SerializeField] public TextMeshProUGUI[] texts;

    [Header("EndPhase")]
    [SerializeField] ScriptableItem item;
    [SerializeField] public GameObject clearMessage;

    public void CheckAnswer()
    {
        for (int i = 0; i < answers.Length; i++)
        {
            if (answers[i] != texts[i].text)
            {
                return;
            }
        }
        
        Manager.Inventory.RemoveItem("CookieBox", 1);

        if (item != null)
        {
            Manager.Inventory.ObtainItem(item);
        }
        

        if (clearMessage != null)
        {
            clearMessage.SetActive(true);
        }
    }
}
