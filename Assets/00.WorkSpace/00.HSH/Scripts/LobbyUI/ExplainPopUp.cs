using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplainPopUp : PopUpUI
{
    [SerializeField] GameObject blocker;
    [SerializeField] GameObject text;
    protected override void Start()
    {
        base.Start();
        Manager.Text.TextChange();
        ForcingRead();
    }
    IEnumerator Blocker()
    {
        blocker.SetActive(true);
        yield return new WaitForSeconds(1f);
        blocker.SetActive(false);
        Manager.Data.UserGameData.SetData(UserGameData.GameDataEnum.LobbyInfo, false);
    }
    public void ForcingRead()
    {
        StartCoroutine(Blocker());
    }
}
