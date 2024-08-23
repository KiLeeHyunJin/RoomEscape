using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 빌드 파일 실행시 Debug.Log 대용. 멤버변수로 txt있으니 대입하면 해당 문구와 같이 
/// 창 출력 창 클릭시 닫기
/// </summary>
public class StatePopup : PopUpUI
{
    public TextMeshProUGUI txt;

    protected override void Start()
    {
        GetComponent<Button>().onClick.AddListener(Close);
    }
}
