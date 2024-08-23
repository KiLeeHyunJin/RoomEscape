using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class BoxPuzzle : PopUpUI
{
    [SerializeField] ScriptableItem puzzleReward;
    [SerializeField] BoxPuzzleButton[] buttons;
    [SerializeField] GameObject clearMessage;
    [SerializeField] int changeState;

    public static BoxPuzzle Instance;

    protected override void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        ResetButtons();
    }

    private void ResetButtons() // 초기화버튼
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].SetPressed(false); // 모든 버튼을 초기화
        }
    }

    public void CheckPuzzle()
    {
        // 정답 상태 확인
        int[] correctPressedIndices = { 3, 4, 5, 10, 11 };

        for (int i = 0; i < buttons.Length; i++)
        {
            bool shouldBePressed = ((IList<int>)correctPressedIndices).Contains(i);
            //Debug.Log($"버튼 체크 {i}: {shouldBePressed},  {buttons[i].IsPressed()}");

            if (buttons[i].IsPressed() != shouldBePressed)
            {
                //Debug.Log($"버튼 {i} 일치. {shouldBePressed}, {buttons[i].IsPressed()}");
                return; // 정답이 아닌 경우 체크 종료
            }
        }

        // 퍼즐 해결 시 호출할 함수
        CheckAnswer();
    }

    public void CheckAnswer()
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
    }

    public void CloseThis()
    {
        Close();
    }
}
