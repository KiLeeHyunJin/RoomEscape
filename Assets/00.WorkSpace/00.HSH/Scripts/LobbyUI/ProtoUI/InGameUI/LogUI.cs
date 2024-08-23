using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class LogUI : PopUpUI
{
    // 이하 인게임 text 출력용 스크립트
    [Header("Text")]
    [SerializeField] int logNumber;
    [SerializeField] int nameNumber;
    [SerializeField] int _SentenceNum;

    // 일단 제시된 대화 번호를 인트값으로 변경하기 위한 변수
    [Header("StringtoInt")]
    [SerializeField] int sentenceIndex = -1;
    [SerializeField] string logs;
    [SerializeField] string names;
    [SerializeField] List<int> LogNumList;
    [SerializeField] List<int> SpeakerNumList;
    [SerializeField] float speedFactor1;
    [SerializeField] float speedFactor2;
    public List<Sentence> Scenes = new List<Sentence>();

    [SerializeField] TextMeshProUGUI personNameText;
    [SerializeField] TextMeshProUGUI logText;

    protected override  void Start()
    {
        Manager.Game.Pause();
        SetScene(Manager.Text._SentenceNum);
    }
    private State state = State.COMPLETED;
    private enum State
    {
        PLAYING, COMPLETED
    }
    public bool IsCompleted()
    {
        return state == State.COMPLETED;
    }
    public bool IsLastSentence()
    {
        return sentenceIndex + 1 == Scenes.Count;
    }
    private IEnumerator TypeText(string text)
    {
        StringBuilder sb = new StringBuilder();
        sb.Clear();
        logText.text = sb.ToString();
        //logUI.logTextMesh.text = sb.ToString();
        state = State.PLAYING;
        int wordIndex = 0;

        while (state != State.COMPLETED)
        {
            sb.Append(text[wordIndex]);
            logText.text = sb.ToString();
            //logUI.logTextMesh.text = sb.ToString();
            yield return new WaitForSeconds(speedFactor1);
            if (++wordIndex == text.Length)
            {
                yield return new WaitForSeconds(speedFactor2);
                state = State.COMPLETED;
                break;
            }
        }
    }
    public void SetScene(int sceneNumber)
    {
        BackendChartData.logChart.TryGetValue(sceneNumber, out LogChartData logChartData);
        _SentenceNum = sceneNumber;
        //textCSV = CSVReader.Read("LanguageData");
        SetLogList(_SentenceNum);
        SetSpeakerList(_SentenceNum);
        SetSentece(SpeakerNumList, LogNumList);
        PlayScene();
    }
    public void SetLogList(int sceneNumber)
    {
        BackendChartData.logChart.TryGetValue(_SentenceNum, out LogChartData logChartData);
        LogNumList = new List<int>();
        //string str = textCSV[sceneNumber]["Korean"].ToString();
        string str = logChartData.korean;
        string[] splitted = str.Split(", ");
        foreach (string s in splitted)
        {
            LogNumList.Add(int.Parse(s));
        }
    }
    public void SetSpeakerList(int sceneNumber)
    {
        BackendChartData.logChart.TryGetValue(_SentenceNum, out LogChartData logChartData);
        SpeakerNumList = new List<int>();
        //string[] splitted = textCSV[sceneNumber]["English"].ToString().Split(", ");
        string[] splitted = logChartData.english.Split(", ");
        foreach (string s in splitted)
        {
            SpeakerNumList.Add(int.Parse(s));
        }
    }

    public struct Sentence
    {
        public string Speaker;
        public string Log;
    }
    public void SetSentece(List<int> speaker, List<int> Log)
    {
        Scenes = new List<Sentence>();

        for (int i = 0; i < speaker.Count; i++)
        {
            if (Manager.Text._Iskr == true)
            {
                Sentence s = new Sentence();
                BackendChartData.logChart.TryGetValue(speaker[i], out LogChartData logChartData);
                s.Speaker = logChartData.korean.ToString();
                //s.Speaker = textCSV[speaker[i]]["Korean"].ToString();
                //s.Log = textCSV[Log[i]]["Korean"].ToString();
                BackendChartData.logChart.TryGetValue(Log[i], out LogChartData logData);
                s.Log = logData.korean.ToString();
                Scenes.Add(s);
            }
            else
            {
                Sentence s = new Sentence();
                BackendChartData.logChart.TryGetValue(speaker[i], out LogChartData logChartData);
                s.Speaker = logChartData.english.ToString();
                //s.Speaker = textCSV[speaker[i]]["English"].ToString();
                //s.Log = textCSV[Log[i]]["English"].ToString();
                BackendChartData.logChart.TryGetValue(Log[i], out LogChartData logData);
                s.Log = logData.english.ToString();
                Scenes.Add(s);
            }
        }
    }
    public void PlayScene()
    {
        sentenceIndex++;
        personNameText.text = Scenes[sentenceIndex].Speaker;
        //logUI.nameTextMesh.text = Scenes[sentenceIndex].Speaker;
        StartCoroutine(TypeText(Scenes[sentenceIndex].Log));
    }
    public void NextAct()
    {
        if (IsCompleted())
        {
            if (IsLastSentence())
            {
                Scenes.Clear();
                LogNumList = new List<int>();
                SpeakerNumList = new List<int>();
                sentenceIndex = -1;
                Manager.Game.Resume();
                Manager.UI.ClosePopUpUI();
                Manager.Chapter._clickObject.AfterLog();
            }
            else
            {
                PlayScene();
            }
        }
    }
    // 존재하는 모든 텍스트 메쉬 찾아서 배열에 넣고 한영 맞춰서 바꿔주는 스크립트
    // 모든 TextMeshProUGUI를 저장할 배열
    private TextMeshProUGUI[] allText;
    public void ChangeLanguage()
    {
        allText = FindObjectsOfType<TextMeshProUGUI>();

        // 배열에 저장된 모든 TextMeshProUGUI 돌아가면서 한영체크
        for (int i = 0; i < allText.Length; i++)
        {
            if (int.TryParse(allText[i].name, out int value))
            {
                BackendChartData.logChart.TryGetValue(int.Parse(allText[i].name), out LogChartData logChartData);
                if (Manager.Text._Iskr == true)
                {
                    allText[i].text = logChartData.korean;
                }
                else
                {
                    allText[i].text = logChartData.english;
                }
            }
            else
            {
                continue;
            }
        }
    }
    public void NextClick()
    {
        NextAct();
    }
    public void SkipMsg()
    {
        Manager.Game.Resume();
        Manager.UI.ClosePopUpUI();
        Manager.Chapter._clickObject.AfterLog();
    }
}