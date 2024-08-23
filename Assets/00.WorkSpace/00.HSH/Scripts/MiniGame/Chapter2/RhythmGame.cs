using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RhythmGame : PopUpUI
{
    [SerializeField] TimingGame[] buttons;
    [SerializeField] GameObject _endGround;
    [SerializeField] TextMeshProUGUI _endMsg;
    [SerializeField] int _time;
    [SerializeField] GameObject _readyGround;
    [SerializeField] TextMeshProUGUI _readyMSG;
    [SerializeField] GameObject _gameGround;
    public int changeState;

    [SerializeField] int _endMsgFailNum;
    [SerializeField] int _endMsgtryNum;

    // 리듬게임 만들때 이 스크립트 그대로 쓰지말고 상속해서 override로 게임 결과만 구현할 것
    protected override void Start()
    {
        StartCoroutine(startTimer());
    }
    public void Update()
    {
        CheckAnswer();
    }
    public void CheckAnswer()
    {
        for(int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i]._clicked == true)
            {
                continue;
            }
            else
            {
                return;
            }
        }
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i]._success == true)
            {
                continue;
            }
            else
            {
                _endGround.SetActive(true);
                EndMsgFail();
                Debug.Log("fail");
                return;
            }
        }
        _endGround.SetActive(true);
        _endMsg.text = "Success!"; //  이 부분 데이터 테이블 필요
        Debug.Log("success");
        GameEffect();
    }
    private IEnumerator startTimer()
    {
        MsgTry();
        yield return new WaitForSeconds(1);
        for (int i = _time; i > 0; i--)
        {
            _readyMSG.text = ($"{i}");
            yield return new WaitForSeconds(1);
        }
        _readyMSG.text = ($"Ready!");// 이 부분 데이터테이블 추가 필요함
        yield return new WaitForSeconds(1);
        _readyGround.SetActive(false);
        _gameGround.SetActive(true);
    }
    public virtual void GameEffect()
    {

    }
    private void MsgTry()
    {
        Debug.Log("try");
        BackendChartData.logChart.TryGetValue(_endMsgtryNum, out LogChartData logChartData);
        if (logChartData != null)
        {
            Debug.Log($"{_endMsgtryNum}");
            if (Manager.Text._Iskr == true)
            {
                Debug.Log($"{_endMsgtryNum}");
                _readyMSG.text = logChartData.korean;
            }
            else
            {
                _readyMSG.text = logChartData.english;
            }
        }
    }
    private void EndMsgFail()
    {
        BackendChartData.logChart.TryGetValue(_endMsgFailNum, out LogChartData logChartData);
        if (logChartData != null)
        {
            Debug.Log($"{_endMsgFailNum}");
            if (Manager.Text._Iskr == true)
            {
                Debug.Log($"{_endMsgFailNum}");
                _endMsg.text = logChartData.korean;
            }
            else
            {
                _endMsg.text = logChartData.english;
            }
        }
    }
    private void ReadyMsg()
    {

    }
}
