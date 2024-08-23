using UnityEngine;
using UnityEngine.UI;

public class InputColorChangeGame : PopUpUI
{

    [SerializeField] private int[] dials = new int[5];
    [SerializeField] int maxState;
    [SerializeField] int[] correctAnswer;
    [SerializeField] Color[] colors;
    [SerializeField] DialGameButton[] CookieButton;

    [Header("EndPhase")]
    [SerializeField] ScriptableItem treasure;
    public int changeState;
    public GameObject clearMessage;
    [SerializeField] bool checkGet;
    [SerializeField] bool checkQuestion;
    [SerializeField] int questionNum;
    private void OnEnable()
    {
        ChangeColor();
    }
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
        CheckAnswer();
        ChangeColor();
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
        if (clearMessage != null)
        {
            clearMessage.SetActive(true);
        }
        GetItem();
        Manager.Chapter._clickObject.state = changeState;
        ChangeQuestionClear();
        Manager.Chapter._clickObject?.ChangeImage();
        GameEffect();
    }
    private void ChangeColor()
    {
        for (int i = 0; i < CookieButton.Length; i++)
        {
            switch (CookieButton[i].state)
            {
                case 0:
                    CookieButton[i].GetComponent<Image>().color = colors[0];
                    break;
                case 1:
                    CookieButton[i].GetComponent<Image>().color = colors[1];
                    break;
                case 2:
                    CookieButton[i].GetComponent<Image>().color = colors[2];
                    break;
            }
        }
    }
    private void ChangeQuestionClear()
    {
        if (checkQuestion == true)
        {
            Manager.Chapter.HintData.SetClearQuestion(questionNum);
        }
    }
    private void GetItem()
    {
        if (checkGet)
        {
            Manager.Chapter._clickObject.GetItem(Manager.Chapter._clickObject.item);
        }
    }
    public virtual void GameEffect()
    {
        // 아이템, 상태, 클리어 메세지 활성화 등은 위에서 지정해놨으나 혹시 추가 효과 필요하면 이 클래스 상속하고 이 함수에 작성
    }
}
