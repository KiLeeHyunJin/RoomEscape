using BackEnd;
using TMPro;
using UnityEngine;

public class UserGameDataPanel : MonoBehaviour
{
    [SerializeField] TMP_Text hint;
    [SerializeField] TMP_Text gold;
    [SerializeField] TMP_Text ticket;
    [SerializeField] TMP_Text jewel;

    private void Awake()
    {
       //LoadData();가 끝날 때 유저정보 패널도 최신화
       Manager.Data.onJsonDataLoadEvent.AddListener(UpdateUserGameData);
    }

    /// <summary>
    /// 불러온 유저정보를 패널에 표기 
    /// </summary>
    public void UpdateUserGameData()
    {
        //hint.text = $"힌트 : {Manager.Data.UserGameData.hint}";
        //gold.text = $"{Manager.Data.UserGameData.gold}";
        //ticket.text = $"{Manager.Data.UserGameData.ticket}";
        //jewel.text = $"{Manager.Data.UserGameData.jewel}";
    }
}
