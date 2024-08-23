using BackEnd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UserGameData;

public class DeleteGuestInfo : MonoBehaviour
{
    private void Awake()
    {
        if (TryGetComponent<Button>(out Button btn))
            btn.onClick.AddListener(Delete);
    }
    //로컬 게스트 정보 삭제 (테스트용)
    public void Delete()
    {
        //Backend.BMember.WithdrawAccount();
        Backend.BMember.DeleteGuestInfo();
       // Manager.Data.UserGameData.SetData(GameDataEnum.Chapter, 0);
    }
}
