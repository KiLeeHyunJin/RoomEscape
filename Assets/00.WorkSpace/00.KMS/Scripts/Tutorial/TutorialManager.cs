using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : PopUpUI
{
    public GameObject[] tutorialPanels; // 각 튜토리얼 패널
    private int currentPanelIndex = 0;

    protected override void Start()
    {
        ShowCurrentPanel();
    }

    public void NextPanel() // 다음 패널로 이동
    {
        if (currentPanelIndex < tutorialPanels.Length - 1)
        {
            currentPanelIndex++;
            ShowCurrentPanel();
        }
        else
        {
            EndTutorial();
        }
    }

    private void ShowCurrentPanel() // 현재 패널
    {
        for (int i = 0; i < tutorialPanels.Length; i++)
        {
            tutorialPanels[i].SetActive(i == currentPanelIndex);
            Manager.Text.TextChange();
        }
    }

    private void EndTutorial()
    {
        foreach (GameObject panel in tutorialPanels)
        {
            panel.SetActive(false);
        }
    }
}
