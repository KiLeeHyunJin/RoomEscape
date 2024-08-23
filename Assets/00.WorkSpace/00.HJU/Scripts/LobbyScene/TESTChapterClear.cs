using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UserGameData;

public class TESTChapterClear : MonoBehaviour
{
    [SerializeField] UserGameDataPanel userGameDataPanel;


    public void NextChapter()
    {
        //Manager.Data.UserGameData.stage = Manager.Data.UserGameData.stage >= 5 ? 1 : ++Manager.Data.UserGameData.stage;
        //Manager.Data.UserGameData.SetData(GameDataEnum.Stage, Manager.Data.UserGameData.stage); 

        Debug.Log($"{Manager.Data.UserGameData.localChartID}");
        ////챕터 클리어 시 다음챕터
        //Manager.Data.UserGameData.chapter += 1;
        //Manager.Data.UserGameData.ticket -= 1;

        //if (Manager.Data.UserGameData.chapter >= 6)
        //{
        //    Manager.Data.UserGameData.chapter = 1;
        //    Manager.Data.UserGameData.stage += 1;
        //}

        //if (Manager.Data.UserGameData.ticket <= 0) 
        //{
        //    Manager.Data.UserGameData.ticket = 5;
        //}

        //유저정보패널 업데이트
        userGameDataPanel.UpdateUserGameData();
    }
}
