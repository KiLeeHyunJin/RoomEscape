using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class ProtoTextManager : MonoBehaviour
{
    [SerializeField] ProtoStorySceneDatabase ProtoSSDatabase;
    [SerializeField] int LogNumber;
    [SerializeField] int NameNumber;
    public ProtoBottomBarUI bottomBarUIPrefab;

    public TextMeshProUGUI personNameText;
    public TextMeshProUGUI LogText;
    public bool isKr;


    public List<Dictionary<string, object>> textCSV;

    public ProtoStoryScene currentScene;
    private State state = State.COMPLETED;
    private enum State
    {
        PLAYING, COMPLETED
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (IsCompleted())
            {
                //Manager.UI.ClosePopUpUI();
                //currentScene = currentScene.nextScene;
                //bottomBar.PlayScene(currentScene);
            }
        }
    }
    public void LoadSentenceUI()
    {
        gameObject.SetActive(true);
        //ProtoStoryScene protoScene = ProtoSSDatabase.protoScenes[LogRow];
        //Manager.UI.ShowPopUpUI(bottomBarUIPrefab);
        //currentScene = protoScene;
    }
    public void SetttingLogNum(int LogNum) // 첫번
    {
        LogNumber = LogNum;
    }
    public void SettingNameNum(int NameNum) // 첫번
    {
        NameNumber = NameNum;
    }
    public void LogSetting() //2번
    {
        textCSV = CSVReader.Read("LanguageData");
        LoadSentenceUI();
        PlaySentence(LogNumber);
        PlaySpeaker(NameNumber);
    }
    public void PlaySentence(int LogRow)
    {
        if ( isKr == true)
        {
            StartCoroutine(TypeText(textCSV[LogRow]["Korean"].ToString()));
           
        }
        else
        {
            StartCoroutine(TypeText(textCSV[LogRow]["English"].ToString()));
        }
    }
    public void PlaySpeaker(int NameRow)
    {
        if (isKr == true)
        {
            personNameText.text = textCSV[NameRow]["Korean"].ToString();
        }
        else
        {
            personNameText.text = textCSV[NameRow]["English"].ToString();
        }
    }
    public bool IsCompleted()
    {
        return state == State.COMPLETED;
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
}