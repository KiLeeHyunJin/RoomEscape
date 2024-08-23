using BackEnd;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GetHash : MonoBehaviour
{
    [SerializeField] TMP_InputField m_Text;

    //해시코드 불러오기 함수, 뒤끝콘솔에 인증정보 입력하기 위한 코드
    public void Hash()
    {
        m_Text.text = Backend.Utils.GetGoogleHash();
    }
}
