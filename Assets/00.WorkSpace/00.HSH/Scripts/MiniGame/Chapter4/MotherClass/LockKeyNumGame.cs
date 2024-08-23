using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

public class LockKeyNumGame : PopUpUI
{
    public string[] letters; // 표시할 문자열 배열
    [Header("ConnectObject")]
    public GameObject nextObject;

    [Header("answer")]
    public string[] answers;
    
    [Header("text")]
    [SerializeField] public TextMeshProUGUI[] texts;
    [SerializeField] int[] states;

    [Header("EndPhase")]
    public ScriptableItem treasure; // 깨면 보상으로 줄 아이템
    public GameObject clearMessage; // 클리어 메세지
    public int changeState; // 깨면 바꿀 클릭오브젝트의 상태번호
    public bool checkGet; // 아이템 먹게 할지 여부
    [SerializeField] bool changeQuestion; // 퀴즈 클리어로 바꿀지 여부
    [SerializeField] int questionNum; // 퀴즈 번호
    
    public void CheckAnswer()
    {
        for (int i = 0; i < answers.Length; i++)
        {
            if (answers[i] != texts[i].text)
            {
                return;
            }
        }
        if (checkGet) // 지금은 클릭오브젝트에 보상 넣어놓고 먹는걸로 굳어져있음 사실상 현재 안쓰이는 부분
        {
            Manager.Chapter._clickObject.GetItem(Manager.Chapter._clickObject.item);
        }
        if (clearMessage != null)
        {
            clearMessage.SetActive(true);
        }
        Manager.Chapter._clickObject.state = changeState;
        ChangeQuestionClear();
        Manager.Chapter._clickObject?.ChangeImage(); // 클릭오브젝트에 바뀔 이미지 잇으면 바뀌고 없으면 말게 해놓음
    }
    public void ChangeNumber(int i)
    {
        states[i] = (states[i] + 1) % letters.Length;
        Debug.Log(states[i]);
        texts[i].text = letters[states[i]];
        CheckAnswer();
    }
    private void ChangeQuestionClear()
    {
        if ( changeQuestion == true)
        {
            Manager.Chapter.HintData.SetClearQuestion(questionNum);
        }
    }
}
