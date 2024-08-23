using BackEnd;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetNickNamePopup : PopUpUI
{
    enum InputField
    {
        NickNameSetInput
    }

    enum Buttons
    {
        NickNameSetButton
    }
    enum Texts
    {
        NickNameSetFailMessage
    }

    TextMeshProUGUI nickNameSetFailMessage;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindInputField(typeof(InputField));
        BindText(typeof(Texts));
        BindButton(typeof(Buttons));
        return true;
    }

    protected override void Start()
    {
        base.Start();
        nickNameSetFailMessage = GetText((int)Texts.NickNameSetFailMessage);
        GetButton((int)Buttons.NickNameSetButton).onClick.AddListener(() =>
        { 
            UpdateNickName(GetInputField((int)InputField.NickNameSetInput).text); 
        });
    }
    public void UpdateNickName(string nickname)
    {
        Backend.BMember.UpdateNickname(nickname, callback =>
        {
            //닉네임생성 성공
            if (callback.IsSuccess())
            {
                Debug.Log($"닉네임 설정에 성공했습니다 : {callback}");

                //계정 생성에 성공했을 때 해당 계정에 게임 정보 생성 
                BackendGameData.Instance.GameDataInsert();

                //모든 차트 데이터 불러오기 
                BackendChartData.LoadAllChart();

                //로비씬 이동
                Manager.UI.ShowScene("LobbyRect");
            }

            //닉네임생성 실패 - 실패 원인 메세지 표기
            else
            {
                switch (callback.GetErrorCode())
                {
                    case "UndefinedParameterException":
                        nickNameSetFailMessage.text = "닉네임을 입력해 주세요.";
                        break;
                    case "BadParameterException":
                        nickNameSetFailMessage.text = "닉네임이 20자를 초과하거나 공란이 존재합니다.";
                        break;
                    case "DuplicatedParameterException":
                        nickNameSetFailMessage.text = "이미 존재하는 닉네임입니다.";
                        break;
                }

            }
        });
    }
}
