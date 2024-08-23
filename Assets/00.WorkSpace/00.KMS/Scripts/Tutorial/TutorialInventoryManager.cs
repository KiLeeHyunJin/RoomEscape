using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialInventoryManager : MonoBehaviour
{
    public GameObject[] tutorialInventoryPanels; // 각 튜토리얼 패널
    private int currentPanelIndex = 0;

    private void Start()
    {
        ShowCurrentPanel();
    }

    public void NextPanel() // 다음 패널로 이동
    {
        if (currentPanelIndex < tutorialInventoryPanels.Length - 1)
        {
            currentPanelIndex++;
            ShowCurrentPanel();
        }
        else
        {
            Manager.Text.TextChange();
            EndTutorial();
        }
    }

    private void ShowCurrentPanel()
    {
        for (int i = 0; i < tutorialInventoryPanels.Length; i++)
        {
            if (i == currentPanelIndex)
            {
                tutorialInventoryPanels[i].SetActive(true);
                Manager.Text.TextChange();
            }
            else
            {
                tutorialInventoryPanels[i].SetActive(false);
            }
        }
    }

    private void EndTutorial()
    {
        foreach (GameObject panel in tutorialInventoryPanels)
        {
            panel.SetActive(false);
        }
    }
}
