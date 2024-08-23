using System.Collections.Generic;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotEnoughPopup: PopUpUI
{
    [SerializeField] Button advBtn;
    [SerializeField] Button purchaseBtn;
    [SerializeField] Button closeBtn;
    
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI info;

    [Header("textNum")]
    [SerializeField] int[] logNumbers;
    [SerializeField] int advertise;
    [SerializeField] int buy;
    [SerializeField] int close;
    [SerializeField] int needsComment;
    [SerializeField] int reset;
    [SerializeField] int limit;
    [SerializeField] int limitComment;
    [SerializeField] int resetQuestion;
    [SerializeField] int limitCountMsg;
    [SerializeField] int resetRange;
    /// <summary>
    /// 매개변수 타입에 맞춰서 창 콜백과 내용 초기화
    /// </summary>
    public void InitState(InitType initType)
    {
        TextMeshProUGUI btnTxt;

        advBtn.onClick = new();
        purchaseBtn.onClick = new();
        closeBtn.onClick = new();

        switch (initType)
        {
            //힌트 개수 부족 시
            case InitType.NotEnough:
                advBtn.onClick.AddListener(Adv);
                btnTxt = advBtn.GetComponentInChildren<TextMeshProUGUI>();
                if (btnTxt != null)
                    btnTxt.text = Manager.Text.TextMake(advertise);

                purchaseBtn.onClick.AddListener(Purchase);
                btnTxt = purchaseBtn.GetComponentInChildren<TextMeshProUGUI>();
                if (btnTxt != null)
                    btnTxt.text = Manager.Text.TextMake(buy);

                closeBtn.onClick.AddListener(Close);
                btnTxt = closeBtn.GetComponentInChildren<TextMeshProUGUI>();
                if (btnTxt != null)
                    btnTxt.text = Manager.Text.TextMake(close);

                title.text = Manager.Text.TextMake(buy);
                info.text = Manager.Text.TextMake(needsComment);
                break;


            //힌트 사용 제한 시
            case InitType.Reset:
                advBtn.onClick.AddListener(Manager.Chapter.ResetHintLimit);
                btnTxt = advBtn.GetComponentInChildren<TextMeshProUGUI>();
                if(btnTxt != null)
                    btnTxt.text = Manager.Text.TextMake(reset);

                purchaseBtn.onClick.AddListener(Close);
                btnTxt = purchaseBtn.GetComponentInChildren<TextMeshProUGUI>();
                if (btnTxt != null)
                    btnTxt.text = Manager.Text.TextMake(close);

                closeBtn.gameObject.SetActive(false);
                title.text = Manager.Text.TextMake(limit);
                info.text = 
                    $"{Manager.Text.TextMake(limitComment)}\n" +
                    $"{Manager.Text.TextMake(resetQuestion)}\n" +
                    $"{Manager.Text.TextMake(limitCountMsg)} : {Manager.Chapter.LimitHintUseCount}" +
                    $"{Manager.Text.TextMake(resetRange)} : {Manager.Chapter.LimitHintQuestionIdx} - {Manager.Chapter.HintData.GetQuestionCount()}";
                break;
        }
    }

    /// <summary>
    /// 광고창을 띄운다
    /// </summary>
    private void Adv()
    {
        AddHintAndClose();
        Utils.ShowAdvertise();
    }
    /// <summary>
    /// 구매 메소드
    /// </summary>
    private void Purchase()
    {
        AddHintAndClose();
    }
    /// <summary>
    /// 힌트 지급 후 종료 매소드
    /// </summary>
    private void AddHintAndClose()
    {
        Manager.UI.ClosePopUpUI();
        Manager.Data.UserGameData.hint++;
    }

    public enum InitType
    {
        NotEnough,
        Reset
    }

}
