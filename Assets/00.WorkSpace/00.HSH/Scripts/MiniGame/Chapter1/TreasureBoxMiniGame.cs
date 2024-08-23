using TMPro;
using UnityEngine;

public class TreasureBoxMiniGame : PopUpUI
{
    public string[] numbers = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" }; // 표시할 문자열 배열
    
    
    [Header("answer")]
    [SerializeField] string[] answers;

    [Header("text")]
    [SerializeField] TextMeshProUGUI[] texts;

    [Header("Item")]
    [SerializeField] public ScriptableItem lockKeyGameItem;

    public void CheckAnswer()
    {
        for(int i = 0; answers.Length > i; i++)
        {
            if (answers[i] == texts[i].text)
            {
                Debug.Log("Successs");
            }
            else
            {
                Debug.Log("Fail");
                return;
            }
        }
    }
    
}
