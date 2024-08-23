using System;
using TMPro;
using UnityEngine;
using static UserGameData;

public class HintPopup : PopUpUI
{
    [SerializeField] TMP_Text text;
    [SerializeField] NotEnoughPopup notEnough;
    Action action;
    protected override void Start()
    {
        //Manager.Data.UserGameData.SetData(GameDataEnum.Hint, 5);
        //text.text = $"X  {Manager.Data.UserGameData.hint}";
    }
    protected void OnEnable()
    {
        Manager.Text.TextChange();
        showHintCount();
    }
    public void SetHintAction(Action _action)
    {
        action = _action;
    }
    public void UseHint()
    {
        if(Manager.Chapter.CheckLimitHintState)
        {
            NotEnoughPopup popUp = Manager.UI.ShowPopUpUI(notEnough);
            if(popUp == null)
            {
                popUp.InitState(NotEnoughPopup.InitType.Reset);
            }
        }
        else
        {
            UsedHint();
        }
    }

    void UsedHint()
    {
        if (Manager.Data.UserGameData.hint > 0)
        {
            Manager.Data.UserGameData.SetData(GameDataEnum.Hint, Manager.Data.UserGameData.hint - 1);
            text.text = $"X  {Manager.Data.UserGameData.hint}";
            //힌트 사용효과 추가
            //Manager.Chapter.hint.data.checkOpened = true;
            action?.Invoke();
            Close();
        }
    }

    public void ShowPopup()
    {
        gameObject.SetActive(true);
    }
    public void ShowAdv()
    {
        Utils.ShowAdvertise();
        //Manager.Data.UserGameData.hint++;
    }
    private void showHintCount()
    {
        text.text = $"X {Manager.Data.UserGameData.hint}";
        Debug.Log("Newest");
    }
}
