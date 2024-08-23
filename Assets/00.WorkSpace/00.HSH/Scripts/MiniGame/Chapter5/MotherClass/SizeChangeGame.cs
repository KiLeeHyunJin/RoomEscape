using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SizeChangeGame : MonoBehaviour
{
    [SerializeField] RectTransform[] circles;
    [SerializeField] int[] state;
    [SerializeField] int[] answer;
    [SerializeField] int maxState;

    // 상속받는 클래스의 Start 에서 stateChangeCheck, changeState, giveItemCheck  선언해줄 것.
    [Header("Effect")]
    [SerializeField] bool StateChangeCheck; // 트루일 때 게임 끝내면 state 변화
    public bool stateChangeCheck 
    {  get { return StateChangeCheck; }
       protected set { StateChangeCheck = value; }
    }
    [SerializeField] int ChangeState;
    public int changeState  // 바꿀 state값
    { 
        get { return ChangeState; } 
        protected set { ChangeState = value; }
    }
    [SerializeField] bool GiveItemCheck; // 트루일 때 게임 끝내면 인벤토리에 treasure 추가
    public bool giveItemCheck
    {
        get { return GiveItemCheck; }
        protected set { GiveItemCheck = value; }
    }
    [SerializeField] ScriptableItem Treasure;
    public ScriptableItem treasure { get { return Treasure; } }

    [SerializeField] GameObject ClearMessage;
    private void Awake()
    {
        for(int i = 0; i < state.Length; i++)
        {
            state[i] = 0;
        }
    }
    [SerializeField] bool QuestionClearCheck; // 트루일 때 게임 끝내면 state 변화
    public bool questionClearCheck
    {
        get { return QuestionClearCheck; }
        protected set { QuestionClearCheck = value; }
    }
    [SerializeField] int QuestionNum;
    public int questionNum  // 바꿀 question id
    {
        get { return QuestionNum; }
        protected set { QuestionNum = value; }
    }
    public void Click(int i)
    {
        // 주의 저 i 값은 0부터 시작해야함
        Debug.Log($"00. {state[i]}");
        state[i]++;
        if (state[i] >= maxState)
        {
            state[i] = state[i] % maxState;
            Debug.Log($"01. {state[i]}");
            Debug.Log($"02. {maxState}");
        }
        Changesprite(i, state[i]);
    }
    private void Changesprite(int i, int state)
    {
        switch(state)
        {
            case 0:
                circles[i].localScale = new Vector2(0.25f, 0.25f);
                break;
            case 1:
                circles[i].localScale = new Vector2(0.5f, 0.5f);
                break;
            case 2:
                circles[i].localScale = new Vector2(0.75f, 0.75f);
                break;
            case 3:
                circles[i].localScale = new Vector2(1f, 1f);
                break;
        }
        CheckAnswer();
    }
    private void CheckAnswer()
    {
        for (int i =0; i < state.Length; i++)
        {
            if (answer[i] != state[i])
            {
                return;
            }
        }
        GameEffect();
    }
    public virtual void GameEffect() 
    {
        ClearMessage.SetActive(true);
        if (giveItemCheck == true)
        {
            Manager.Inventory.ObtainItem(treasure);
        }
        if (stateChangeCheck == true)
        {
            Manager.Chapter._clickObject.state = changeState;
        }
    }
}
