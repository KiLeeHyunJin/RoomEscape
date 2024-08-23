using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class PlayInGameUI : MonoBehaviour
{
    [SerializeField] PopUpUI HintUI;
    [SerializeField] PopUpUI OptionUI;
    [SerializeField] ClickObject click;

    public void ShowHintUI()
    {
        Manager.UI.ShowPopUpUI(HintUI);
    }

    public void ShowOptionUI()
    {
        Manager.UI.ShowPopUpUI(OptionUI);
        if (click == null)
            return;
        click.state = 0;
        click.ClickEffect();
    }

    // 이하 인게임 text 출력용 스크립트
    [Header("Text")]
    [SerializeField] int logNumber;
    [SerializeField] int nameNumber;
    [SerializeField] int _SentenceNum;

    [Header("StringtoInt")]
    [SerializeField] int sentenceIndex = -1;
    [SerializeField] string logs;
    [SerializeField] string names;
    [SerializeField] List<int> LogNumList;
    [SerializeField] List<int> SpeakerNumList;
    [SerializeField] float speedFactor;
    public List<Sentence> Scenes = new List<Sentence>();


    [SerializeField] GameObject LogUI;

    public TextMeshProUGUI personNameText;
    public TextMeshProUGUI LogText;

    public bool isKr;

    public List<Dictionary<string, object>> textCSV;

    private State state = State.COMPLETED;
    private enum State
    {
        PLAYING, COMPLETED
    }
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
    //    {
    //        if (IsCompleted())
    //        {
    //            if (IsLastSentence())
    //            {
    //                Scenes.Clear();
    //                LogNumList = new List<int>();
    //                SpeakerNumList = new List<int>();
    //                sentenceIndex = -1;
    //            }
    //            else
    //            {
    //                PlayScene();
    //            }
    //        }
    //    }
    //}
    public void LoadSentenceUI()
    {
        //ProtoStoryScene protoScene = ProtoSSDatabase.protoScenes[LogRow];
        LogUI.SetActive(true);
        //currentScene = protoScene;
    }
    public void SetttingLogNum(int LogNum) // 첫번
    {
        logNumber = LogNum;
    }
    public void SettingNameNum(int NameNum) // 첫번
    {
        nameNumber = NameNum;
    }
    public void LogSetting() //2번
    {
        textCSV = CSVReader.Read("LanguageData");
        LoadSentenceUI();
        PlaySentence(logNumber);
        PlaySpeaker(nameNumber);
    }
    public void PlaySentence(int LogRow)
    {
        //BackendChartData.logChart.TryGetValue(LogRow, out LogChartData logChartData);
        if (isKr == true)
        {
            StartCoroutine(TypeText(textCSV[LogRow]["Korean"].ToString()));
            //StartCoroutine(TypeText(logChartData.korean));
        }
        else
        {
            StartCoroutine(TypeText(textCSV[LogRow]["English"].ToString()));
            //StartCoroutine(TypeText(logChartData.english));
        }
    }
    public void PlaySpeaker(int NameRow)
    {
        //BackendChartData.logChart.TryGetValue(NameRow, out LogChartData logChartData);
        if (isKr == true)
        {
            personNameText.text = textCSV[NameRow]["Korean"].ToString();
            //personNameText.text = logChartData.korean;
        }
        else
        {
            personNameText.text = textCSV[NameRow]["English"].ToString();
            //personNameText.text = logChartData.english;
        }
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
        LogText.text = sb.ToString();
        Debug.Log($"03. {LogText.text}");
        state = State.PLAYING;
        int wordIndex = 0;

        while (state != State.COMPLETED)
        {
            sb.Append(text[wordIndex]);
            LogText.text = sb.ToString();
            Debug.Log($"04. {LogText.text}");
            yield return new WaitForSeconds(0.05f);
            if (++wordIndex == text.Length)
            {
                yield return new WaitForSeconds(1.5f);
                state = State.COMPLETED;
                break;
            }
        }
    }
    // 이하 씬 재생구조로 변형중------------------------------------------ 이상 건드리지 말것
    //public void StringToint(string num)
    //{
    //    LogNumList = new List<int>();
    //    string[] splitted = num.Split(", ");
    //    foreach (string s in splitted)
    //    {
    //        LogNumList.Add(int.Parse(s));
    //    }
    //    Debug.Log($"00. {LogNumList[0]}");
    //    Debug.Log($"01. {LogNumList[1]}");
    //}
    //public void LogSetting(string num) // Log지정 0번
    //{
    //    SavesceneLog(num, -1, true);
    //}
    //public void SpeakerSetting(string num) // 화자지정 0번
    //{
    //    SaveSceneSpeaker(num, -1, true);
    //}
    //public void SavesceneLog(string num, int sentenceIndex = -1, bool isAnimated = true) // 1번
    //{
    //    LogNumList = new List<int>();
    //    string[] splitted = num.Split(", ");
    //    foreach (string s in splitted)
    //    {
    //        LogNumList.Add(int.Parse(s));
    //    }
    //    this.sentenceIndex = sentenceIndex;
    //    logSetOver = true;
    //    //PlaySentence2(isAnimated);
    //}
    //public void SaveSceneSpeaker(string num, int speakerIndex = -1, bool isAnimated = true) // 화자지정 1번
    //{
    //    SpeakerNumList = new List<int>();
    //    string[] splitted = num.Split(", ");
    //    foreach (string s in splitted)
    //    {
    //        SpeakerNumList.Add(int.Parse(s));
    //    }
    //    this.speakerIndex = speakerIndex;
    //    //PlaySpeaker2(isAnimated);
    //    SpeakerSetOver = true;
    //}
    //public void PlaySentence2(bool isAnimated = true) //2번
    //{
    //    LogUI.SetActive(true);
    //    if (isKr == true)
    //    {
    //        speakerIndex++;
    //        personNameText.text = textCSV[SpeakerNumList[speakerIndex]]["Korean"].ToString();
    //        sentenceIndex++;
    //        speedFactor = 1f;
    //        StartCoroutine(TypeText(textCSV[LogNumList[sentenceIndex]]["Korean"].ToString()));
    //    }
    //    else
    //    {
    //        speakerIndex++;
    //        personNameText.text = textCSV[SpeakerNumList[speakerIndex]]["English"].ToString();
    //        sentenceIndex++;
    //        speedFactor = 1f;
    //        Debug.Log($"00. {sentenceIndex}");
    //        Debug.Log($"01. {LogNumList[sentenceIndex]}");
    //        StartCoroutine(TypeText(textCSV[LogNumList[sentenceIndex]]["English"].ToString()));
    //    }
    //}
    //public void PlayNextSentence(bool isAnimated = true) // 3번
    //{
    //    sentenceIndex++;
    //    PlaySentence2(isAnimated);
    //}

    // 인트값 하나로 다 해결하고 싶은데 
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
            if (isKr == true)
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
        LogUI.SetActive(true);
        sentenceIndex++;
        personNameText.text = Scenes[sentenceIndex].Speaker;
        speedFactor = 1f;
        StartCoroutine(TypeText(Scenes[sentenceIndex].Log));
    }
    //public bool CheckQuestion()
    //{
    //    //for (int i = 0; i < Manager.Chapter.QuestionDataBases[Manager.Chapter.chapter].Questions.Length; i++)
    //    //{
    //    //    if (Manager.Chapter.QuestionDataBases[Manager.Chapter.chapter].Questions[i].cleared == false)
    //    //    {
    //    //        return false;
    //    //    }
    //    //    else
    //    //    {
    //    //        continue;
    //    //    }
    //    //}
    //    //return true;
    //}
}
