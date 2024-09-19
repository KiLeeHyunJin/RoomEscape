using System.Collections;
using System.Text;
using UnityEngine;
using Unity;

public class GameManager : Singleton<GameManager>
{


    [SerializeField] PopUpUI gameOverPopUpUI;
    [SerializeField] PopUpUI gameClearPopUpUI;
    //제한시간
    public int LimitTime { get { return limitTime; } }
    int limitTime;
    public bool IsReady { get; private set; } = true;
    public bool Clear{ get { return isClear; } }
    public bool Over{ get { return isOver; } }
    public bool GiveUp { get { return isGiveUp; } }
    public bool Save { get { return isSave; } }
    bool isCoolAbsState = false;
    bool isGiveUp;
    bool isClear;
    bool isOver;
    bool isSave;
    [SerializeField] PopUpUI inGameBackButtonUI;
    [SerializeField] GameObject outGameBackButtonUI;

    const int minAbsCool = 1;

    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
    }
    //필드에 배치할 위치
    [SerializeField] Transform Field;
    Coroutine timerCo;
    // 매개변수가  자식으로 들어와야하고 앞에 있어야하는지 판단 후 객층 위치 설정
    public void ObjectSetParentToFront(Transform target, bool state = true)
        => SetParent(target, Field, state);

    void SetParent(Transform target, Transform parent, bool state)
    {
        target.SetParent(parent);
        if (state)
            target.SetAsLastSibling();
        else
            target.SetAsFirstSibling();
    }
    private void OnEnable()
    {
        Init();
    }

    public void ShowCoolTimeAbs()
    {
        if(Manager.Data.GetOpenAlbumCount() >= 2 && isCoolAbsState == false)
        {
            Debug.Log("Call Cool Ads");
            isCoolAbsState = true;
            Advertise adv = Utils.ShowAdvertise();
            StartCoroutine(AbsCoolRoutine());
            //if (adv != null)
            //    adv.CoolState();
        }
    }
    public void ShowNewAbs()
    {
        if (Manager.Data.GetOpenAlbumCount() >= 2)
        {
            if(isCoolAbsState == false)
            {
                isCoolAbsState = true;
                StartCoroutine(AbsCoolRoutine());
            }
            Debug.Log("Call New Ads");
            Utils.ShowAdvertise();
        }
    }
    IEnumerator AbsCoolRoutine()
    {
        yield return new WaitForSecondsRealtime(minAbsCool * 60);
        isCoolAbsState = false;
    }


    public void SetGiveUp()
    {
        isGiveUp = true;
    }

    //초기화
    void Init()
    {
        Field = null;
        StopTimer();
    }
    public StringBuilder limitTimeString;
    //배치 위치 설정
    public void SetField(Transform field)
    {
        Field = field;
    }
    //제한 시간 설정 및 타이머 시작
    public void TimerStart(int time = 90)
    {
        isOver = false;
        isClear = false;
        isGiveUp = false;
        //Manager.Data.SetChapterData(Manager.Chapter.chapter, (null, true));
        limitTime = time;
        Resume();
    }
    //일시정지 해제(타이머 코루틴 실행)
    public void Resume()
    {
        StopTimer();
        timerCo = StartCoroutine(TimeCheckRoutine());
    }
    //일시정지(타이머 코루틴 종료)
    public void Pause()
    {
        StopTimer();
    }
    //코루틴을 종료
    void StopTimer()
    {
        IsReady = true;
        if (timerCo != null)
            StopCoroutine(timerCo);
    }
    public void GameClear()
    {
        isClear = true;
        int chapterNum = Manager.Chapter.chapter;
        if (chapterNum > 0)
            Manager.Data.SetChapterData(chapterNum, (true, null));
        Init();
    }

    // 시간 추가
    public void AddTime(int addTime)
    {
        limitTime += addTime;
    }

    // 시간 차감
    public void DeductionTime(int deductionTime)
    {
        limitTime -= deductionTime;
    }

    IEnumerator TimeCheckRoutine()
    {
        IsReady = false;
        limitTimeString ??= new StringBuilder(10);
        while (LimitTime > 0)
        {
            limitTime--;
            int min = (limitTime / 60);
            int sec = (limitTime % 60);
            limitTimeString.Clear();
            limitTimeString.Append(string.Format("{0:D2} : {1:D2}", min, sec));

//#if UNITY_EDITOR
//            Debug.Log(limitTimeString.ToString());
//#endif
            yield return new WaitForSeconds(1);   
        }
        GameOver();
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause == false)
        {
            ShowCoolTimeAbs();
        }
    }

    void GameOver()
    {
        isOver = true;
        Init();
        Manager.UI.ShowPopUpUI(gameOverPopUpUI);
    }

    public void GameClearPopup()
    {
        Manager.UI.ShowPopUpUI(gameClearPopUpUI);
    }

    public void Vibration()
    {
#if UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }

    private void Update()
    {
        AndroidBackButton();
    }

    public void AndroidBackButton()
    {
#if UNITY_ANDROID
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            if (currentSceneName == "InGameScene")
            {
                Manager.UI.ShowPopUpUI(inGameBackButtonUI);
            }
            else
            {
                outGameBackButtonUI.SetActive(true);
            }
        }
#endif
    }

    public void BackButtonUIQuit()
    {
        Application.Quit();
    }

    public void BackButtonUICancel()
    {
        outGameBackButtonUI.SetActive(false);
    }
}
