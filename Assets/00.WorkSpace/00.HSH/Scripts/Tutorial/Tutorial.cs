using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class Tutorial : PopUpUI
{
    [SerializeField] GameObject part1;
    [SerializeField] GameObject part2;
    [SerializeField] GameObject part3;
    [SerializeField] GameObject part4;

    public void Firststep()
    {
        Manager.Chapter.chapter = 0;
        part2.SetActive(true);
        part1.SetActive(false);
        Manager.Text.TextChange();
    }
    public void Secondstep()
    {
        //Manager.Chapter.QuestionObjectCheck();
        Debug.Log($"chapterNum : {Manager.Chapter.chapter}");
        part3.SetActive(true);
        part2.SetActive(false);
        Manager.Text.TextChange();
    }
    public void Thirdstep()
    {
        part4.SetActive(true);
        part3.SetActive(false);
        Manager.Text.TextChange();
    }
    public void FirthStep()
    {
        Manager.UI.ClearPopUpUI();
        Manager.Scene.LoadScene("Chapter0Scene");
    }
}
