using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlidingPuzzle : PopUpUI
{
    [SerializeField] GameObject clearMessage;
    [SerializeField] int changeState;
    [SerializeField] ScriptableItem puzzleReward;
    public RectTransform[] puzzleObjects; // 퍼즐 오브젝트
    public RectTransform[] targetPositions; // 목표 위치
    public RectTransform emptyPosition; // 빈 공간 위치
    [SerializeField] bool changeQuestion;
    [SerializeField] int questionNum;

    protected override  void Start()
    {
        if (puzzleObjects == null || puzzleObjects.Length == 0)
        {
            //Debug.LogError("퍼즐 오브젝트가 설정되지 않았습니다.");
            return;
        }

        if (targetPositions == null || targetPositions.Length == 0)
        {
            //Debug.LogError("목표 위치가 설정되지 않았습니다.");
            return;
        }

    }

    public void CheckAnswer()
    {
        bool isComplete = true;

        for (int i = 0; i < puzzleObjects.Length; i++)
        {
            if (Vector3.Distance(puzzleObjects[i].anchoredPosition, targetPositions[i].anchoredPosition) > 0.1f)
            {
                //Debug.Log(Vector3.Distance(puzzleObjects[i].anchoredPosition, targetPositions[i].anchoredPosition));
                isComplete = false;
                break;
            }
        }

        if (isComplete)
        {
            //Debug.Log("퍼즐 완료!");
            GameEffect();
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
            Manager.Chapter._clickObject.state = changeState;
            Manager.Chapter._clickObject.ChangeImage();
        }
        if(changeQuestion == true)
        {
            Manager.Chapter.HintData.SetClearQuestion(questionNum);
        }
    }

    public virtual void GameEffect()
    {

    }
}
