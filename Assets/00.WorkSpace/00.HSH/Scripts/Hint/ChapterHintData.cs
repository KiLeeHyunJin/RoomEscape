using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChapterHintData
{
    QuestionDataBaseObject currentChpaterHintDataBase;
    readonly ChapterManager owner;
    [SerializeField] List<Question> dataList;
    public HintSaveData saveData;
    [SerializeField] int currentDataChapterNum;

    [field : SerializeField] int limitHintQuestionIdx;
    [field: SerializeField] int limitHintUseCount;
    public int LimitHintQuestionIdx { get { return limitHintQuestionIdx; } }
    public int LimitHintUseCount { get { return limitHintUseCount; } }
    //bool loadCompelte;

    // ChapterManager 스타트에서 초기화
    // dataList 는 question 의 리스트
    public ChapterHintData(ChapterManager _owner)
    {
        owner = _owner;
        //loadCompelte = false;
        Init();
        dataList = new();
    }
    
    // 챕터 로드 후 LoadGameTable 이 스타트에서 로드 힌트 데이터
    public void Init(QuestionDataBaseObject _currentChpaterHintDataBase)
    {
        currentChpaterHintDataBase = _currentChpaterHintDataBase;
        limitHintQuestionIdx = default;
        limitHintUseCount = default;

        if (currentChpaterHintDataBase == null)
            Debug.LogWarning("HintData is Null");

        limitHintQuestionIdx = currentChpaterHintDataBase.limitHintQuestionIdx;
        limitHintUseCount = currentChpaterHintDataBase.limitHintCount;

        if (co != null)
            owner.StopCoroutine(co);
        co = owner.StartCoroutine(WaitCurrentChapterData());
    }
    Coroutine co;
    IEnumerator WaitCurrentChapterData()
    {
        while
            (currentDataChapterNum != Manager.Chapter.chapter || 
             currentChpaterHintDataBase == null)
        {
            yield return null;
        }
        PushData();
    }

    public void Init()
    {
        //loadCompelte = false;
        currentChpaterHintDataBase = null;
    }
    // 데이터리스트 작성

    public bool CheckLimitUseHint()
    {
        if (GetQuestionState(limitHintQuestionIdx))
        {
            //Message.LogWarning("힌트 사용 제한 구역입니다.");

            if (GetOpenCheckLastToInputValue(limitHintQuestionIdx) > limitHintUseCount)
            {
                //Message.LogError($"" +
                //    $"{limitHintUseCount}개 이상의 힌트가 열려있습니다. \n" +
                //    $"현재 힌트 사용 제한 구역의 힌트를 초기화 하시겠습니까?");

                return true;
            }
        }
        return false;
    }

    public int ResetHintAll()
    {
        return ResetHintUsedState(0, GetQuestionCount());
    }
    public int ResetHintLimit()
    {
        int start = Mathf.Min(limitHintQuestionIdx, GetQuestionCount());
        int last = Mathf.Max(limitHintQuestionIdx, GetQuestionCount());
        return ResetHintUsedState(start, last);
    }

    public int ResetHintUsedState(int start, int last)
    {
        int resetCount = default;


        if (int.Equals(start, last))
        {
            for (int j = 0; j < dataList[start].hints.Count; j++)
            {
                if (dataList[start].hints[j].opened)
                {
                    dataList[start].hints[j].opened = false;
                    resetCount++;
                }
            }
        }
        else
        {
            for (int i = start; i < last; i++)
            {
                for (int j = 0; j < dataList[i].hints.Count; j++)
                {
                    if (dataList[i].hints[j].opened)
                    {
                        dataList[i].hints[j].opened = false;
                        resetCount++;
                    }
                }
            }
        }
        return resetCount;
    }
    void PushData()
    {
        //Debug.Log("PushData");
        int bundleSize = currentChpaterHintDataBase.Questions.Length;
        // question list 를 클리어하고 
        if (dataList == null)
            dataList = new(bundleSize);
        else
        {
            if (dataList.Capacity < bundleSize)
                dataList.Capacity = bundleSize;
            dataList.Clear();
        }

        for (int i = 0; i < bundleSize; i++)
            dataList.Add(new Question());

        for (int i = 0; i < bundleSize; i++)
        {
            QuestionObject question = currentChpaterHintDataBase.Questions[i];

            if (question.hints == null)
                continue;

            int questionHintSize = question.hints.Length;
            bool state = default;
            bool clear = default;
            for (int j = 0; j < questionHintSize; j++)
            {
                ObjectSaveData[] objectSaveData = saveData.objectState;

                if (objectSaveData != null &&
                    objectSaveData.Length > i)
                {
                    clear = objectSaveData[i].objectState;
                    if (objectSaveData[i].hintSaveData != null &&
                        objectSaveData[i].hintSaveData.Length > j)
                    {
                        state = objectSaveData[i].hintSaveData[j];
                    }
                }
                dataList[i].cleared = clear;
                dataList[i].hints.Add(new(state, question.hints[j]));
            }
        }
    }

    public void Load(string json, List<bool> loadClearList, int chapterNum)
    {
        if(string.IsNullOrEmpty(json))
        {
            LoadNull(chapterNum);
            return;
        }
        saveData = JsonUtility.FromJson<HintSaveData>(json);

        if (loadClearList != null)
        {
            for (int i = 0; i < loadClearList.Count; i++)
            {
                if (saveData.objectState.Length <= i)
                    break;
                saveData.objectState[i].objectState = loadClearList[i];
            }
        }
        else
        {
            for (int i = 0; i < saveData.objectState.Length; i++)
                saveData.objectState[i].objectState = false;
        }
        currentDataChapterNum = chapterNum;
    }

    // 힌트 상태 변경
    public void SetOpenHintState(int questionIdx, int hintIdx)
    {
        dataList[questionIdx].hints[hintIdx].opened = true;
    }
    // question clear
    public void SetClearQuestion(int questionIdx)
    {
        dataList[questionIdx].cleared = true;
    }

    // 힌트 상태 얻는곳
    public bool GetOpenHintState(int questionIdx, int hintIdx)
    {
        return dataList[questionIdx].hints[hintIdx].opened;
    }
    public void LoadNull(int chapterNum)
    {
        dataList?.Clear(); //데이터가 없기때문에 기존 데이터 초기화
        saveData.objectState = null;
        currentDataChapterNum = chapterNum;
    }
    // 힌트 갯수 얻기
    public int GetHintCount(int questionIdx)
    {
        if (currentChpaterHintDataBase == null)
            return 0;
        if (currentChpaterHintDataBase.Questions == null)
            return 0;
        if (currentChpaterHintDataBase.Questions[questionIdx].hints == null)
            return 0;
        return currentChpaterHintDataBase.Questions[questionIdx].hints.Length;
    }
    // question 갯수 얻기
    public int GetQuestionCount()
    {
        if (currentChpaterHintDataBase == null)
            return 0;
        if (currentChpaterHintDataBase.Questions == null)
            return 0;
        return currentChpaterHintDataBase.Questions.Length;
    }

    public int GetOpenCheckLastToInputValue(int checkStartValue)
    {
        int temp = default;
        if (checkStartValue >= currentChpaterHintDataBase.Questions.Length)
            return -1;
        QuestionObject[] searchArray = currentChpaterHintDataBase.Questions;
        for (int i = checkStartValue; i < searchArray.Length; i++)
        {
            for (int j = 0; j < searchArray[i].hints.Length; j++)
            {
                if (GetOpenHintState(i, j))
                    temp++;
            }
        }
        return temp;
    }

    // 튜플
    public (Sprite  sprite,string information) GetHint(int questionIdx, int hintIdx)
    {
        HintObject hint = dataList[questionIdx].hints[hintIdx].hint;
        return (hint.Sprite, hint.Information);
    }
    // question 상태 얻는곳
    public bool GetQuestionState(int questionIdx)
    {
        if(dataList.Count <= questionIdx)
        {
            return false;
        }
        return dataList[questionIdx].cleared;
    }

   
    
    public List<bool> SaveData()
    {
        List<bool> clearStateList = new(dataList.Count);
        saveData = new(dataList.Count);
        for (int i = 0; i < dataList.Count; i++)
        {
            List<HintData> questionData = dataList[i].hints;

            int questionHintCount = questionData.Count;
            bool questionState = dataList[i].cleared;

            clearStateList.Add(questionState);

            saveData.objectState[i].objectState = questionState;
            saveData.objectState[i].hintSaveData = new bool[questionHintCount];

            for (int j = 0; j < questionHintCount; j++)
            {
                saveData.objectState[i].hintSaveData[j] = questionData[j].opened;
            }
        }
        return clearStateList;
    }


    [Serializable]
    class Question
    {
        public Question() 
        {
            hints = new();
        }
        public bool cleared;
        public List<HintData> hints;
    }

    [Serializable]
    class HintData
    {
        internal HintData(bool _state, HintObject _hint)
        {
            opened = _state;
            hint = _hint;
        }
        internal HintData()
        {

        }
        internal HintObject hint;
        public bool opened;
    }

    [Serializable]
    public struct HintSaveData
    {
        public HintSaveData(int size)
        {
            objectState = new ObjectSaveData[size];
        }
        public ObjectSaveData[] objectState;
    }
    [Serializable]
    public struct ObjectSaveData
    {
        public bool[] hintSaveData;
        public bool objectState;
    }

}
