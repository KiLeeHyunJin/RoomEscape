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

    }

    /// <summary>
    /// 광고창을 띄운다
    /// </summary>
    private void Adv()
    {
        AddHintAndClose();
       // Utils.ShowAdvertise();
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
        //Manager.Data.UserGameData.hint++;
    }

    public enum InitType
    {
        NotEnough,
        Reset
    }

}
