using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UserGameData;

public class InGameBackButtonUI : PopUpUI
{
    public void ChapterGiveUp(string sceneName)
    {
        Manager.Game.SetGiveUp();
        ReturnLobby(sceneName);
    }

    void ReturnLobby(string sceneName)
    {
        Manager.Data.UserGameData.SetData(GameDataEnum.Chapter, -1);
        Manager.Inventory.Init();
        Manager.UI.ClearPopUpUI();
        Manager.Game.ShowNewAbs();
        Manager.Scene.LoadScene(sceneName);
    }
}
