using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ChapterManager : Singleton<ChapterManager>
{
    [field: SerializeField] public int chapter { get; set; }

    string Path { get { return Manager.Data.DataPath; } }

    // 데이터 전달용
    public int chapterCost = 1;
    public ClickObject _clickObject;
    public ScriptableItem item;
    public GameObject userInventory;
    public Sprite HintPopUpSprite;
    public bool ContinueState { get; set; }
    public int LimitHintQuestionIdx { get { return hintData.LimitHintQuestionIdx; } }
    public int LimitHintUseCount { get { return hintData.LimitHintUseCount; } }

    [SerializeField] ChapterHintData hintData;
    [SerializeField] HintJsonData hintUseData;
    public bool CheckLimitHintState { get { return hintData.CheckLimitUseHint(); } }
    public ChapterHintData HintData { get { return hintData; } }
    public string CheckSaveFile()
    {
        Manager.Chapter.ContinueState = false;



        return null;
    }


    private void Start()
    {
        hintData = new(this);
    }

    //세이브파일이 존재하는지 확인
    public void CheckSaveFile(int chapterNumber, Action<bool> returnMethod)
    {
        ReturnMethod = returnMethod;
    }

    Action<bool> ReturnMethod;


    IEnumerator GetCheckingMethod(int chapterNumber)
    {
        string temp = null;
        int idx = 0; 
        void GetChecking(string datas, int scope)
        {
            temp = datas;
            idx = scope;
        }
        BackendGameData.Instance.ChapterDataLoad(GetChecking, chapterNumber);
        do
        {
            yield return null;
        }
        while (idx == 0);
        if(idx > 0)
        {
            ReturnMethod?.Invoke(string.IsNullOrEmpty(temp) == false);
        }
        else
        {
            ReturnMethod?.Invoke(false);
        }
    }



    //힌트 에셋번들 로드 후 값 설정
    public void LoadHintDataBase(List<bool> clearStatList, int chapterNum)
    {
        if (0 == chapterNum) //튜토리얼일 경우 로드 취소
            return;

        LoadHintData(clearStatList, chapterNum);
        //Message.Log("Hint Load Start");

        Manager.Resource.GetAsset($"chapter0{chapterNum}", "Question Database", ResourceType.Scriptable, (obj) =>
        {
            QuestionDataBaseObject db = obj as QuestionDataBaseObject;
            hintData.Init(db);
        
        }, true, false);
    }


    List<bool> loadClearList;
    int chapterNum;
    void LoadHintData(List<bool> _loadClearList, int _chapterNum)
    {
        loadClearList = _loadClearList;
        chapterNum = _chapterNum;
        BackendGameData.Instance.ChapterDataLoad(LoadHint, chapterNum);
    }
        
    void LoadHint(string getHintValue, int idx)
    {
        if (string.IsNullOrEmpty(getHintValue) == false)
        {

            hintUseData = JsonUtility.FromJson<HintJsonData>(getHintValue);
            HintData.Load(hintUseData.hintDataString, loadClearList, chapterNum);
            Debug.Log($"loading Hint");
        }
        else
            HintData.LoadNull(chapterNum);
    }

    public void ResetHintLimit()
    {
        int resetCount = Manager.Chapter.hintData.ResetHintLimit();
        //Message.LogError($"{resetCount}개의 힌트를 초기화하였습니다.");
    }

    public void ResetHintAll()
    {
        int resetCount = Manager.Chapter.hintData.ResetHintAll();
        //Message.LogError($"{resetCount}개의 힌트를 초기화하였습니다.");
    }

    public List<bool> SaveHintData()
    {
        List<bool> clearList = hintData.SaveData();
        hintUseData.hintDataString = JsonUtility.ToJson(hintData.saveData);
        BackendGameData.Instance.ChapterDataUpdate((BackendGameData.ChapterDataEnum)chapterNum, JsonUtility.ToJson(hintUseData));
        //string chapterHintPath = System.IO.Path.Combine(Path, $"chapterHint_{chapter}.txt");
        //File.WriteAllText(chapterHintPath,);
        return clearList;
    }


    private void OnDestroy()
    {
        ChapterSaveLoad saveLoad = FindObjectOfType<ChapterSaveLoad>();
        if (saveLoad == null)
            return;
        saveLoad.SaveCurrentChapter();
    }
    public struct HintJsonData
    {
        public string hintDataString;
    }
}
