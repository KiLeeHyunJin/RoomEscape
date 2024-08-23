using UnityEngine;
using UnityEngine.UI;

public class DialBoxPopup : PopUpUI
{
    [SerializeField] GameObject clearMessage;
    //[SerializeField] public ClickObject connectObject;
    [SerializeField] private int[] dials = new int[4];
    [SerializeField] ScriptableItem item1;
    [SerializeField] ScriptableItem item2;
    public DialGameButton[] dialButtons;
    [SerializeField] Sprite[] changeSprites;
    [SerializeField] int changeState;
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
        dialButtons[i - 1].state += 1;
        if (dials[i - 1] >= 3)
        {
            dials[i - 1] = 0;
            dialButtons[i - 1].state = 0;
        }
        CheckSprite();
        CheckAnswer();
    }

    public void CheckAnswer()
    {
        int[] correctAnswer = { 1, 2, 1, 2 };
        for (int j = 0; j < dials.Length; j++)
        {
            if (dials[j] != correctAnswer[j])
            {
                return;
            }
        }
        Manager.Chapter._clickObject.state = changeState;
        Manager.Chapter._clickObject.GetItem(item1);
        Debug.Log($"1번 아이템 넣음");
        Manager.Chapter._clickObject.GetItem(item2);
        Debug.Log($"2번 아이템 넣음");
        clearMessage.SetActive(true);
        if ( changeQuestion == true)
        {
            Manager.Chapter.HintData.SetClearQuestion(questionNum);
        }
    }
    private void CheckSprite()
    {
        for (int i = 0; i < dialButtons.Length; i++)
        {
            switch (dialButtons[i].state)
            {
                case 0:
                    //DialGameButton button = dialButtons[i];
                    //GameObject gameObject = button.gameObject;
                    //Image img = gameObject.GetComponent<Image>();

                    //img.sprite = changeSprites[0];
                    dialButtons[i].GetComponent<Image>().sprite = changeSprites[0];
                    break;
                case 1:
                    dialButtons[i].GetComponent<Image>().sprite = changeSprites[1];
                    break;
                case 2:
                    dialButtons[i].GetComponent<Image>().sprite = changeSprites[2];
                    break;
            }
        }
    }
}
