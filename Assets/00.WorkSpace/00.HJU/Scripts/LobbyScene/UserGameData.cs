using BackEnd;
using System;
using UnityEngine;

[System.Serializable]

public class UserGameData 
{
    public string localChartID;
    public int chapter;
    public int hint;
    public int sfx;
    public int bgm;
    public bool kor;
    public bool lobbyInfo;
    public string currentVer;
    public int[] datas;
    public UserGameData()
    {
        datas = new int[(int)GameDataEnum.End];
    }

    public void SetData(GameDataEnum gameDataType, object value)
    {
        switch (gameDataType)
        {
            case GameDataEnum.LocalChartID:
                localChartID = value.ToString();
                break;
            case GameDataEnum.Chapter:
                chapter = Convert.ToInt32(value);
                break;
            case GameDataEnum.Hint:
                hint = Convert.ToInt32(value);
                break;
            case GameDataEnum.Sfx:
                sfx = Convert.ToInt32(value);
                break;
            case GameDataEnum.Bgm:
                bgm = Convert.ToInt32(value);
                break;
            case GameDataEnum.Kor:
                kor = Convert.ToBoolean(value);
                break;
            case GameDataEnum.LobbyInfo:
                lobbyInfo = Convert.ToBoolean(value);
                break;
            case GameDataEnum.CurrentVer:
                currentVer = value.ToString();
                break;
        }
        Manager.Data.UserDataAllSave();
    }

    public void Reset()
    {
        //Utils.ShowInfo("Reset");
        localChartID = null;
        chapter = 0;
        hint = 3;
        sfx = -20;
        bgm = -20;
#if UNITY_ANDROID && UNITY_EDITOR == false
        SystemLanguage language = Application.systemLanguage;
        if(language == SystemLanguage.Korean)
        {
                kor = true;
                Manager.Text._Iskr = true;
                Manager.Text.TextChange();
        }
        else
        {
                kor = false;
                Manager.Text._Iskr = false;
                Manager.Text.TextChange();
        }
#else
        kor = true;
#endif
        lobbyInfo = true;
        currentVer = null;
    }

    public enum GameDataEnum
    {
        LocalChartID,
        Chapter,
        Hint,
        Sfx,
        Bgm,
        Kor,
        CurrentVer,
        LobbyInfo,
        End
    }

}
