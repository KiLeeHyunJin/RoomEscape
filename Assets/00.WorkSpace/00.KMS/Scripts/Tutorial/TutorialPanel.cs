using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour
{
    private Button nextButton;
    private TutorialManager tutorialManager;

    private void Start()
    {
        tutorialManager = FindObjectOfType<TutorialManager>();
        nextButton = GetComponentInChildren<Button>();
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(tutorialManager.NextPanel);
        }
        Manager.Text.TextChange();
    }
}
