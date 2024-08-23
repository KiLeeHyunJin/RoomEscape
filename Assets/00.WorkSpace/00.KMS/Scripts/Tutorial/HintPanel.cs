using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HintPanel : PopUpUI
{
    public GameObject hintTutorialPanel;
    public GameObject hintConfirmPanel;
    public GameObject hintPopUp;

    public Button hintButton;
    public Button confirmButton;
    public Button yesButton;
    public Button noButton;

    private HintPopup hintPopupComponent;

    protected override void Start()
    {
        hintTutorialPanel.SetActive(false); // 초기에는 튜토리얼 패널 숨기기
        hintConfirmPanel.SetActive(false); // 초기에는 확인 패널 숨기기
        hintPopUp.SetActive(false);

        hintButton.onClick.AddListener(ShowHintTutorial);
        confirmButton.onClick.AddListener(OnConfirmButtonClick);
        yesButton.onClick.AddListener(OnYesButtonClick);
        noButton.onClick.AddListener(OnNoButtonClick);
        hintPopupComponent = hintPopUp.GetComponent<HintPopup>(); // HintPopup 컴포넌트 가져오기
    }

    void ShowHintTutorial()
    {
        HintActiveState(true);
        hintPopUp.SetActive(false);

        hintTutorialPanel.SetActive(true); // 튜토리얼 패널 보여주기
    }

    void OnConfirmButtonClick()
    {

        HintActiveState(false);
        hintPopUp.SetActive(true);
        hintPopupComponent?.ShowPopup(); // HintPopup 컴포넌트의 팝업 보여주기 메서드 호출

        hintTutorialPanel.SetActive(false); // 튜토리얼 패널 숨기기
        hintConfirmPanel.SetActive(false); // 확인 패널 숨기기

    }

    void OnYesButtonClick()
    {
        hintPopUp.SetActive(true);
        hintConfirmPanel.SetActive(false); // 확인 패널 숨기기
    }

    void OnNoButtonClick()
    {
        hintConfirmPanel.SetActive(false); // 확인 패널 숨기기
        hintPopUp.SetActive(false);
    }

    void HintActiveState(bool state)
    {
        hintTutorialPanel.SetActive(state); // 튜토리얼 패널 보여주기
        hintConfirmPanel.SetActive(state); // 확인 패널 보여주기

    }

    private void OnButtonState(bool state)
    {
        hintTutorialPanel.SetActive(state);
        hintConfirmPanel.SetActive(state);
    }
}
