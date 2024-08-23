using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] int time = 1200;
    [SerializeField] int addTime = 30;
    [SerializeField] int deductionTime = 30;
    [SerializeField] bool check = false;
    void Start()
    {
        check = false;
        //Manager.Chapter.QuestionObjectCheck();
        if (Manager.Chapter.ContinueState == false)
        {
            switch(Manager.Chapter.chapter)
            {
                case 0:
                    break;
                case 1:
                    time = 900;
                    // 15분
                    break;
                case 2:
                    time = 1200;
                    // 20분
                    break;
                case 3:
                    time = 1200;
                    // 20분
                    break;
                case 4:
                    time = 1500;
                    // 25분
                    break;
                case 5:
                    time = 1800;
                    // 30분
                    break;
            }
            Manager.Game.TimerStart(time);
        }
        
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        if(Manager.Game.IsReady == false)
            text.text = Manager.Game.limitTimeString.ToString();

        if (!check && Manager.Game.LimitTime <= 120)
        {
            check = true;
            Manager.Sound.PlaySoundFromTime(6, 18);
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
