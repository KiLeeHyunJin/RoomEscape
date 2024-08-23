using TMPro;
using UnityEngine;

public class VarietyUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI chapterName;
    [SerializeField] TextMeshProUGUI stageName;

    [SerializeField] PopUpUI AlbumUIPrefab;
    [SerializeField] GameObject stageUI;
    [SerializeField] GameObject chapterUI;
    [SerializeField] GameObject[] stages;
    public int chapternum;
    private int stagePhaseNum;

    public void ChoiceChapterName(string name)
    {
        chapterName.text = name;
    }
    public void ChoiceChapterNum(int num)
    {
        chapternum = num;
    }
    public void ChoiceStage(string name)
    {
        stageName.text = name;
    }
    public void ChoiceStageNum(int num)
    {
        stagePhaseNum = num;
    }
    public void GoToStageUI()
    {
        if( chapternum > -1)
        {
            stageUI.SetActive(true);
            stages[chapternum].SetActive(true);
            chapterUI.SetActive(false);
        }
        else
        {
            return;
        }
    }
    public void BackToChapter()
    {
        chapterUI.SetActive(true);
        stages[chapternum].SetActive(false);
        stageUI.SetActive(false);
    }
    public void DiveInstage()
    {
        if(stagePhaseNum > -1)
        {
            Debug.Log($"{stagePhaseNum} 스테이지로 이동");
        }
        else
        {
            return;
        }
    }
    public void PopUpAlbumUI()
    {
        Manager.UI.ShowPopUpUI(AlbumUIPrefab);
    }
}
