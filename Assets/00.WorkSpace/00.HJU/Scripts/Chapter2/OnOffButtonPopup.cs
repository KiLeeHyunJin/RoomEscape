using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OnOffButtonPopup : MonoBehaviour
{
    [SerializeField] GameObject clearMessage;
    [SerializeField] private int[] correctButtons;
    [SerializeField] Sprite[] changeSprites;
    private Button[] buttons;
    [SerializeField] bool changeQuestion;
    [SerializeField] int questionNum;
    private void Start()
    {
        buttons = new Button[transform.childCount];
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i] = transform.GetChild(i).GetComponent<Button>();
            int index = i; 
            buttons[i].onClick.AddListener(() => OnButtonClick(index));
        }
    }

    private void OnButtonClick(int index)
    {
        Image buttonImage = buttons[index].GetComponent<Image>();
        buttonImage.sprite = buttonImage.sprite == changeSprites[0] ? changeSprites[1] : changeSprites[0];

        CheckAnswer();
    }

    private void CheckAnswer()
    {
        // 모든 버튼이 정확한 상태인지 확인
        for (int i = 0; i < buttons.Length; i++)
        {
            Image buttonImage = buttons[i].GetComponent<Image>();

            // correctButtons에 포함된 버튼은 On 상태여야 하고, 포함되지 않은 버튼은 Off 상태여야 함
            if (correctButtons.Contains(i))
            {
                if (buttonImage.sprite != changeSprites[1]) // On 상태
                {
                    return;
                }
            }
            else
            {
                if (buttonImage.sprite != changeSprites[0]) // Off 상태
                {
                    return;
                }
            }
        }
        if ( changeQuestion == true)
        {
            Manager.Chapter.HintData.SetClearQuestion(questionNum);
        }
        clearMessage.SetActive(true);
        Manager.Chapter._clickObject.GetItem(Manager.Chapter._clickObject.item);
        Manager.Chapter._clickObject.state = 1;
    }
}
