using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ChapterSaveLoad : MonoBehaviour
{
    [SerializeField] int chapterNum;
    [SerializeField] ChapterData chapterData;
    [SerializeField] bool saveState;
    bool isSavingDataFinder;
    //bool isBackGround;

    string Path { get { return Manager.Data.DataPath; } }
    private void Start()
    {
        //isBackGround = false;
        saveState = false;
        isSavingDataFinder = false;

        int chapterNum = Manager.Chapter.chapter;
        Manager.Data.UserGameData.SetData(UserGameData.GameDataEnum.Chapter, chapterNum);

    }

    /// <summary>
    /// 챕터매니저에서 현재 챕터 넘버를 가져온다.
    /// </summary>
    [ContextMenu("RefreshChapterNumber")]
    public void LoadChapterNum() 
    {
        chapterNum = Manager.Chapter.chapter;
    }

    /// <summary>
    /// 해당 챕터의 데이터를 저장한다.
    /// </summary>
    [ContextMenu("SaveLoad/Save")]
    public void SaveCurrentChapter()
    {
        LoadChapterNum(); 
        if (saveState) 
            return;

        saveState = true; //저장상태
        if (Manager.Game.GiveUp || Manager.Game.Clear || Manager.Game.Over) //종료
            return;

        if (chapterNum == 0) //튜토리얼
            return;

        BackendGameData.Instance.GameDataUpdate(); //서버에 데이터 저장
        Manager.Data.SaveChapterClear();//챕터 클리어 데이터 저장
        chapterData.Init(); //현재 챕터 데이터 구조체 값 초기화
        //재귀를 통해 클릭 오브젝트 값 저장
        Recursion(transform, (clickObject) => 
        {
            chapterData.state.Add(clickObject.state);
            chapterData.activeState.Add(clickObject.gameObject.activeSelf);
            if (isSavingDataFinder == false)
            {
                if (clickObject.state > 0)
                {
                    isSavingDataFinder = true;
                }
            }
        });

        if (isSavingDataFinder == false) //저장할 요소가 없으면 종료
            return;

        //제한 시간 및 인벤토리 데이터 저장
        chapterData.time = Manager.Game.LimitTime;
        chapterData.inventoryString = Manager.Inventory.SaveInventory();
        chapterData.clearList = Manager.Chapter.SaveHintData();

        string chapterDatastring = JsonUtility.ToJson(chapterData);
        BackendGameData.Instance.ChapterDataUpdate(BackendGameData.ChapterDataEnum.chapterData, chapterDatastring);
    }


    /// <summary>
    ///  해당 챕터의 데이터를 로드한다.
    /// </summary>
    [ContextMenu("SaveLoad/Load")]
    public void LoadCurrentChpater(Action<List<bool>>getStatList)
    {
        CallGetState = getStatList;
        BackendGameData.Instance.ChapterDataLoad(LoadCurrentChapterData);
    }

    Action<List<bool>> CallGetState;

    void LoadCurrentChapterData(string json, int idx)
    {
        if (string.IsNullOrEmpty(json))
        {
            CallGetState?.Invoke(null);
            return;
        }

        chapterData = JsonUtility.FromJson<ChapterData>(json);
        CallGetState?.Invoke(chapterData.clearList);

        //해당 챕터의 번호 불러오기
        LoadChapterNum();

        if(Manager.Chapter.ContinueState)
        {
            //재귀를 통한 클릭 오브젝트 상태 대입
            int checkNumIdx = default;
            Recursion(transform, (clickObject) =>
            {
                clickObject.state = chapterData.state[checkNumIdx];
                clickObject.gameObject.SetActive(chapterData.activeState[checkNumIdx++]);
            });
            //시간 대입 및 인벤토리 데이터 대입
            Manager.Game.TimerStart(chapterData.time);
            Manager.Inventory.LoadInventory(chapterData.inventoryString, chapterNum);
        }

       
    }



    /// <summary>
    /// 재귀를 하면서 클릭오브젝트면 액션함수를 실행한다.
    /// </summary>
    void Recursion(Transform parent, Action<ClickObject> work)
    {
        foreach (Transform child in parent)
        {
            if (child.TryGetComponent<ClickObject>(out ClickObject clickObject))
            {
                work?.Invoke(clickObject);
            }
            Recursion(child, work);
        }
    }

    /// <summary>
    ///세이브 파일이 존재하면 삭제
    ////// </summary>
    public void RemoveSaveFile(int chapterNumber)
    {
        Array array = Enum.GetValues(typeof(DataManager.DataName));
        foreach (DataManager.DataName dataName in array)
        {
            //Manager.Data.GetBackendChartName(dataName);

        }
    }


    void OnApplicationPause(bool pauseStatus)
    {
        //백그라운드 시 저장
        if (pauseStatus)
        {
            SaveCurrentChapter();
        }
        else
        {
            if (saveState)
                saveState = false;
        }
    }

    private void OnApplicationQuit()
    {
        //어플 종료시 저장
        SaveCurrentChapter();
    }

    private void OnDestroy()
    {
        //객체 파괴시 저장
        Manager.Chapter.SaveHintData();
    }

    [Serializable]
    public struct ChapterData
    {
        /// <summary>
        /// 값 초기화
        /// </summary>
        public void Init()
        {
            state = new();
            activeState = new();
            time = default;
            inventoryString = null;
        }

        public List<int> state;
        public List<bool> activeState;
        public int time;
        public string inventoryString;
        public List<bool> clearList;

    }
}

public struct SuddenSaveChapter
{
    public int saveChapterNum;
    public bool[] saveActiveState;
    public int[] saveState;
}