using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class DescriptionTextBox : MonoBehaviour
{
    TextMeshProUGUI tmp;
    /// <summary>
    /// 문자열을 텍스트 창에 대입한다.
    /// </summary>
    
    public string Text
    {
        set
        {
            if (tmp == null)
                tmp = GetComponent<TextMeshProUGUI>();
            tmp.text = value;
        }
    }
    private void Awake()
    {
        //GetComponentInChildren<TextMeshProUGUI>()?.FontAsset(Define.Font.CRegular);
    }
}
