using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlbumViewerUI : PopUpUI
{
    [SerializeField] Sprite[] chapterImages;
    [SerializeField] Image viewImage;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI log;
    [SerializeField] GameObject PrevButton;
    [SerializeField] GameObject NextButton;
    [SerializeField] GameObject LastButton;


    [Header("StringtoInt")]
    [SerializeField] int sentenceIndex = -1;
    [SerializeField] string logs;
    [SerializeField] string names;
    [SerializeField] List<int> TitleNumList;
    [SerializeField] List<int> LogNumList;
    private List<AlbumStory> stories = new List<AlbumStory>();
    private StoryState storyState = StoryState.COMPLETED;

    [Header("StoryNumber")]
    [SerializeField] int storyNum;
    [SerializeField] int[] albumStories;
    [Header("ChapterName")]
    [SerializeField] TextMeshProUGUI ChapterTitleName;
    [SerializeField] string[] chapterNames;
    protected override void Awake()
    {
        base.Awake();
        SetChapterName(Manager.Chapter.chapter);
    }
    protected override void Start()
    {
        base.Start();
        ImageChange();
        SetStory(Manager.Chapter.chapter);
        SetButtons();
        Manager.Text.TextChange();
    }
    private void ImageChange()
    {
        viewImage.sprite = chapterImages[Manager.Chapter.chapter];
    }
    private struct AlbumStory
    {
        public string title;
        public string log;
    }
    private enum StoryState
    {
        PLAYING, COMPLETED
    }
    private void SetChapterName(int i)
    {
        ChapterTitleName.name = chapterNames[i];
    }
    void SetStory(int chapterNum)
    {
        SetPlayStoryNum(chapterNum);
        SetTitleList(storyNum);
        SetLogList(storyNum);
        SetStoryList(TitleNumList, LogNumList);
        PlayNextStory();
    }
    private void SetPlayStoryNum(int chapterNum)
    {
        storyNum = albumStories[chapterNum];
    }
    private void SetTitleList(int storyNum)
    {
        BackendChartData.logChart.TryGetValue(storyNum, out LogChartData logChartData);
        TitleNumList = new List<int>();
        string[] splitted = logChartData.english.Split(", ");
        foreach (string s in splitted)
        {
            TitleNumList.Add(int.Parse(s));
        }
    }
    void SetLogList(int storyNum)
    {
        BackendChartData.logChart.TryGetValue(storyNum, out LogChartData logChartData);
        LogNumList = new List<int>();
        string[] splitted = logChartData.korean.Split(", ");
        foreach(string s in splitted)
        {
            LogNumList.Add(int.Parse(s));
        }
    }
    void SetStoryList(List<int> title, List<int> story)
    {
        stories = new List<AlbumStory>();
        for (int i = 0; i < title.Count; i++)
        {
            if (Manager.Text._Iskr == true)
            {
                AlbumStory s = new AlbumStory();
                BackendChartData.logChart.TryGetValue(title[i], out LogChartData logChartData);
                s.title = logChartData.korean.ToString();
                BackendChartData.logChart.TryGetValue(story[i], out LogChartData logData);
                s.log = logData.korean.ToString();
                stories.Add(s);
            }
            else
            {
                AlbumStory s = new AlbumStory();
                BackendChartData.logChart.TryGetValue(title[i], out LogChartData logChartData);
                s.title = logChartData.english.ToString();
                BackendChartData.logChart.TryGetValue(story[i], out LogChartData logData);
                s.log = logData.english.ToString();
                stories.Add(s);
            }
        }
    }
    public void NextAct()
    {
        if (IsLastSentence())
        {
            stories.Clear();
            LogNumList = new List<int>();
            TitleNumList = new List<int>();
            sentenceIndex = -1;
            Manager.Game.ShowCoolTimeAbs();
            if (Manager.Chapter.chapter == 6)
            {
                Manager.UI.ClearPopUpUI();
            }
            else
            {
                Manager.UI.ClosePopUpUI();
            }
        }
        else
        {
            PlayNextStory();
        }
    }
    public void PlayNextStory()
    {
        sentenceIndex++;
        title.text = stories[sentenceIndex].title;
        log.text = stories[sentenceIndex].log;
        SetButtons();
    }
    public void PlayPrevStory()
    {
        sentenceIndex--;
        title.text = stories[sentenceIndex].title;
        log.text = stories[sentenceIndex].log;
        SetButtons();
    }
    private void SetButtons()
    {
        if (sentenceIndex + 1 == stories.Count)
        {
            NextButton.SetActive(false);
            LastButton.SetActive(true);
            Manager.Text.TextChange();
        }
        else
        {
            NextButton.SetActive(true);
            LastButton.SetActive(false);
            Manager.Text.TextChange();
        }
        if (sentenceIndex + 1 < 2)
        {
            PrevButton.SetActive(false);
        }
        else
        {
            PrevButton.SetActive(true);
            Manager.Text.TextChange();
        }
    }
    public bool IsCompleted()
    {
        return storyState == StoryState.COMPLETED;
    }
    public bool IsLastSentence()
    {
        return sentenceIndex + 1 == stories.Count;
    }
    public void CloseAlbumUI()
    {
        stories.Clear();
        LogNumList = new List<int>();
        TitleNumList = new List<int>();
        sentenceIndex = -1;
        Manager.Game.ShowCoolTimeAbs();
        if(Manager.Chapter.chapter == 6)
        {
            Manager.UI.ClearPopUpUI();
        }
        else
        {
            Manager.UI.ClosePopUpUI();
        }
    }
}
