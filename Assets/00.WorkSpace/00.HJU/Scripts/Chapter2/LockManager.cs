using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 각 ScrollLock이 답과 일치하는지 검사하는 매니저
/// </summary>
public class LockManager : PopUpUI
{
    [SerializeField] GameObject clearMessage;
    [SerializeField] private ScrollLock[] scrollLocks;
    [SerializeField] int [] correctNumber;
    [SerializeField] bool questionChange;
    [SerializeField] int questionNum;
    public int[] CorrectNumber => correctNumber;

    protected override  void Start()
    {
        for (int i = 0; i < scrollLocks.Length; i++)
        {
            scrollLocks[i].index = i;
        }
    }

    public void CheckCombination()
    {
        if (CheckAllScrollsCorrect())
        {
            Unlock();
        }
        else
        {
            Debug.Log("Faile");
        }
    }

    private bool CheckAllScrollsCorrect()
    {
        foreach (ScrollLock scrollLock in scrollLocks)
        {
            if (!scrollLock.IsCorrectNumber())
            {
                return false;
            }
        }
        return true;
    }

    private void Unlock()
    {
        clearMessage.SetActive(true);
        Manager.Chapter._clickObject.state++;
        if (Manager.Chapter._clickObject.item != null)
        {
            Manager.Chapter._clickObject.GetItem(Manager.Chapter._clickObject.item);
        }
        if( questionChange )
        {
            Manager.Chapter.HintData.SetClearQuestion(questionNum);
        }
    }
}
