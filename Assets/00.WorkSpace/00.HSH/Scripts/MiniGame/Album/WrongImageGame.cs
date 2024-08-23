using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UserGameData;

public class WrongImageGame : MonoBehaviour, IPointerClickHandler
{
    [Header("Option")]
    [SerializeField] float maxtime = 90;
    [SerializeField] int addTime = 30;
    [SerializeField] int deductionTime = 30;
    [SerializeField] int questCount;
    //[Header("Start")]
    //[SerializeField] Button[] totalQuest;
    //[SerializeField] List<int> randomNum;
    //[SerializeField] Button[] chosenQuest;


    [Header("InGame")]
    public int foundCount;
    [SerializeField] Image[] _Heart;
    [SerializeField] PooledObject _XImage;
    [SerializeField] int life;
    [SerializeField] int hintCoin;
    private Coroutine TimerCoroutine;
    [SerializeField] TextMeshProUGUI countText;

    [Header("random")]
    [SerializeField] WrongImageQuiz[] totalQuest;
    [SerializeField] List<int> randomCount;
    public List<WrongImageQuiz> choosenQuest;

    [Header("Timer")]
    [SerializeField] Slider Timer;
    [SerializeField] float changeTime;
    [SerializeField] float fillTime;
    [SerializeField] Image timerImage;
    [SerializeField] Color timerImageColor;
    [SerializeField] float RGBred;
    [SerializeField] float RGBgreen;
    [SerializeField] float RGBblue;
    [SerializeField] bool check = false;

    [Header("End")]
    [SerializeField] GameObject _ClearMsg;
    [SerializeField] TextMeshProUGUI _EndMsg;
    [SerializeField] string _ClearMsgNum;
    [SerializeField] string _FailMsgNum;
    [SerializeField] GameObject _Blocker;
    [SerializeField] int _hintBonus;

    [Header("Save")]
    [SerializeField] DataManager.GameDataName chapterName;
    [SerializeField] int gameNum;
    protected void Start()
    {
        _ClearMsgNum = "157";
        _FailMsgNum = "158";

        CreateQuestionList();

        Manager.Pool.CreatePool(_XImage, 10, 10, true);
        TimerCoroutine = StartCoroutine(TimeSlideBar());
        foundCount = 0;
        countText.text = "0";
        SetList();
        Manager.Sound.PlayBGMByIndex(9);
    }

    private void Update()
    {
        if (!check && fillTime <= 30)
        {
            check = true;
            Manager.Sound.PlayBGMByIndex(8);
        }
    }

    private void SetList()
    {
        for (int i = 0; i < 10; i++)
        {
            randomCount.Add(i);
        }
        Extension.Shuffle(randomCount);
        for(int i = 0; i < 5; i++)
        {
            choosenQuest.Add(totalQuest[randomCount[i]]);
            choosenQuest[i].gameObject.SetActive(true);
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 screenPosition = eventData.position;
        PooledObject obj = Manager.Pool.GetPool(_XImage);
        (obj.transform as RectTransform).anchoredPosition = screenPosition;
        Manager.UI.ShowEffect(obj.transform);
        LifeCut();
        CheckAnswer();
        Manager.Sound.PlayButtonSound(42);// 나중에 번호 바꿔서 조정할 수 있음
    }
    public void CheckAnswer()
    {
        CountCollect();
        if (life == 0)
        {
            _ClearMsg.SetActive(true);
            _EndMsg.name = _FailMsgNum;
            Manager.Text.TextChange();
            StopCoroutine(TimerCoroutine);
            Manager.Sound.StopBGM();
            Manager.Sound.PlayItemSound(16);
            return;
        }
        if (foundCount < 5)
        {
            return;
        }
        GameEffect();
    }
    private void LifeCut()
    {
        life--;
        _Heart[life].fillAmount = 0;
    }
    private void GameEffect()
    {
        if (_ClearMsg != null)
        {
            _ClearMsg.SetActive(true);
            _EndMsg.name = _ClearMsgNum;
            Manager.Text.TextChange();
            StopCoroutine(TimerCoroutine);
        }
        if (Manager.Data.GetClearAlbumState(chapterName, gameNum )== false)
        {
            int num = Manager.Data.UserGameData.hint + _hintBonus;
            Manager.Data.UserGameData.SetData(GameDataEnum.Hint, num);
            Manager.Data.SetClearAlbum(chapterName, gameNum, true);
        }
        //Debug.Log($"HintCount : {Manager.Data.UserGameData.hint}");
        Manager.Sound.PlayBGMByIndex(7);
        Manager.Sound.PlayItemSound(15);
        Debug.Log($"Manager.Data.GetClearAlbumState(chapterName, gameNum) : {Manager.Data.GetClearAlbumState(chapterName, gameNum)}");
    }
    private void OnDisable()
    {
        Manager.Pool.DestroyPool(_XImage);
        Manager.UI.DelEffectObject();
        Manager.Sound.PlayBGMByIndex(1);
    }
    IEnumerator TimeSlideBar()
    {
        fillTime = maxtime;
        timerImageColor = new Color(RGBred, RGBgreen, RGBblue, 1);

        while (fillTime > 0)
        {
            yield return new WaitForSeconds(changeTime);
            fillTime = fillTime - changeTime;
            Timer.value = fillTime / maxtime;
            RGBChange();
            //Debug.Log($" fillTime : { fillTime }");
            //Debug.Log($"s");
        }
        life = 0;
        CheckAnswer();
    }
    private void RGBChange()
    {
        Debug.Log("first");
        RGBred = (1 - (fillTime / maxtime));
        RGBgreen = fillTime / maxtime;

        timerImageColor = new Color(RGBred, RGBgreen, RGBblue, 1);
        timerImage.color = timerImageColor;
        Debug.Log($"timerImageColor : {timerImageColor}");
    }
    public void WrongGameHint()
    {
        for (int i = 0; i < 5; i++)
        {
            if (choosenQuest[i]._Founded != true)
            {
                // 아직 못찾은거 있으면 힌트 실행
                StartCoroutine(WrongHint(i));
                Debug.Log($"WrongGameHint Answer : {i}");
                return;
            }
        }
    }

    IEnumerator WrongHint(int i)
    {
        if (hintCoin > 0)
        {
            hintCoin--;
            Manager.Sound.PlayButtonSound(17);
            _Blocker.SetActive(true);
            choosenQuest[i].GreenCircle[0].SetActive(true);
            choosenQuest[i].GreenCircle[1].SetActive(true);
            yield return new WaitForSeconds(1.5f);
            _Blocker.SetActive(false);
        }
    }
    private void CountCollect()
    {
        countText.text = foundCount.ToString();
    }

    private void CreateQuestionList()
    {
        Extension.Shuffle(randomCount);
        for(int i = 0;i < questCount;i++)
        {
            choosenQuest[i] = new();
            choosenQuest[i] = totalQuest[randomCount[i]];
        }
    }



    [ContextMenu("일시정지")]
    public void Stop()
    {
        Manager.Game.Pause();
    }

    [ContextMenu("일시정지 해제")]
    public void ReStart()
    {
        Manager.Game.Resume();
    }

    [ContextMenu("시간 추가")]
    public void TimePlus()
    {
        Manager.Game.AddTime(addTime);
    }

    [ContextMenu("시간 차감")]
    public void TimeMinus()
    {
        Manager.Game.DeductionTime(deductionTime);
    }
}
