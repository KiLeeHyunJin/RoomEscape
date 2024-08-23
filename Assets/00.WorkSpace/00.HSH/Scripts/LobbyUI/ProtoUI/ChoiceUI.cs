using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceUI : MonoBehaviour
{
    [SerializeField] GameObject chapterUI;
    [SerializeField] GameObject optionUI;
    [SerializeField] GameObject beforeUI;
    [SerializeField] GameObject middleUI;
    [SerializeField] GameObject afterUI;
    [SerializeField] GameObject[] chapters;
    [SerializeField] GameObject[] middles;
    [SerializeField] Image[] downloadIcon;
    [SerializeField] PlayerBeforeCheckPopup continuePopup;
    [SerializeField] Button playButton;
    public int chapternum;
    public string chapterName;


    private void Start()
    {
        chapternum = -1;

        Manager.Text.TextChange();
        Manager.Data.OptionDataLoad();

        string attachBaseName = "chapter0";
        for (int i = 0; i < downloadIcon.Length; i++)
        {
            int idx = i;
            Manager.DownLoadBundle.HasBundleCheck($"{attachBaseName}{idx + 1}", (state) =>
            {
                if (state)
                    downloadIcon[idx].gameObject.SetActive(false);
            });
        }

        if(Manager.Chapter.ContinueState)
            Manager.Chapter.ContinueState = false;

        int chapterNum = Manager.Data.UserGameData.chapter;
        Utils.ShowInfo($"UserData chapter Data is {chapterNum}");
        if(chapterNum <= 0)//튜토리얼 진행을 위해 or 이어할 게임 없음
        {
            return;
        }
        chapternum = Manager.Chapter.chapter = chapterNum;

        Manager.Data.UserGameData.SetData(UserGameData.GameDataEnum.Chapter, -1);
        
        ContinuePlayeChapter();
    }



    public void ChoiceChapterNum(int i)
    {
        chapternum = i;
        Manager.Chapter.chapter = i;
        beforeUI.SetActive(false);
        //optionUI.SetActive(false);
        middleUI.SetActive(true);
        middles[chapternum].SetActive(true);
        Manager.Text.TextChange();
    }

    public void ChoiceChapterName(string name)
    {
        chapterName = name;
    }

    public void AfterChoice()
    {
        chapters[chapternum].SetActive(true);
        afterUI.SetActive(true);
        middleUI.SetActive(false);
        Debug.Log($"chapterNum : {Manager.Chapter.chapter}");
        Manager.Text.TextChange();
    }
    public void BacktoMain()
    {
        beforeUI.SetActive(true);
        //optionUI.SetActive(true);
        middleUI.SetActive(false);
        middles[chapternum].SetActive(false);
        chapters[chapternum].SetActive(false);
        Manager.Text.TextChange();
    }
    public void BackToMiddle()
    {
        middleUI.SetActive(true);
        afterUI.SetActive(false);
        Manager.Text.TextChange();
    }
    public void ContinuePlayeChapter()
    {
        Manager.Chapter.ContinueState = true;
        string bundleName = $"chapter0{Manager.Chapter.chapter}";
        //Debug.Log($"Check Bundle Num : {bundleName}");
        Manager.DownLoadBundle.HasBundleCheck(bundleName, (state) =>
        {
            if (state) //소지중이라면 
            {
                PlayGame(); //없다면 게임 시작
            }
            else //번들 소지중이 아니라면 다운로드 팝업 실행
            {
                Manager.DownLoadBundle.DownLoad(null, "basic", bundleName);
            }
        });
    }
    public void PlayChapter()
    {
        if (chapternum < 0)
            return;
        Manager.UI.ClearPopUpUI();
        Manager.Chapter.ContinueState = false;
        if (chapternum == 0) //튜토리얼
        {
            //Utils.ShowInfo("chapternum == 0");
            PlayGame();
        }
        else
        {
            string bundleName = $"chapter{string.Format("{0:D2}", chapternum)}";
            Debug.Log($"Check Bundle Num : {bundleName}");
            //Utils.ShowInfo($"Check Bundle Num : {bundleName}");
            //에셋 번들을 소지중인지 확인
            Manager.DownLoadBundle.HasBundleCheck(bundleName, (state) =>
            {
                //Utils.ShowInfo($"Has Bundle State {state}");
                if (state) //소지중이라면 
                {
                    //if (CheckSaveFile() == false) //세이브 파일이 있는지 확인 있다면 컨티뉴 팝업 출력
                    //{
                        PlayGame(); //없다면 게임 시작
                    //}
                }
                else //번들 소지중이 아니라면 다운로드 팝업 실행
                {
                    Manager.DownLoadBundle.DownLoad(null, "basic", bundleName);
                }
            });
        }
    }

    bool CheckSaveFile()
    {
        if (chapternum < 0)
            return false;

        PlayerBeforeCheckPopup continuePopupInstacneUI = Manager.UI.ShowPopUpUI("CheckPopup") as PlayerBeforeCheckPopup; //컨티뉴 팝업 출력
        if (continuePopupInstacneUI != null)
        {
            continuePopupInstacneUI.InitAction(
                () =>
                {
                    Manager.Chapter.ContinueState = true;//데이터 로드
                    EnterGame();
                },
                
                Manager.UI.ClosePopUpUI
                ); //게임실행 메소드 전달
            //Utils.ShowInfo("PlayerBeforeCheckPopup is Show");
        }
        //else
            //Utils.ShowInfo("PlayerBeforeCheckPopup is NUll");
        return true;
    }

    void EnterGame()
    {
        //게임 실행
        PlayGame();
        //팝업 종료
        Manager.UI.ClearPopUpUI();
    }

    void PlayGame()
    {
        //Manager.Chapter.hintInventory.Viewer.UpdateNum();
        Debug.Log($"chapterNum : {Manager.Chapter.chapter}");
        playButton.interactable = false;
        //Manager.Chapter.QuestionObjectCheck();

        Manager.Scene.LoadScene(chapternum == 0 ? chapterName: $"InGameScene");
        //Manager.Scene.LoadScene(chapterName); 
    }
}
