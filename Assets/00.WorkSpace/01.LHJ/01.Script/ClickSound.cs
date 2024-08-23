using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class ClickSound : MonoBehaviour
{
    public int sfxIndex = 0;

    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            //button.onClick = new();
            button.onClick.AddListener(PlayClickSound);
        }
    }

    /// <summary>
    /// 버튼이 클릭되면 사운드 출력
    /// </summary>
    public void PlayClickSound()
    {
        Manager.Sound.PlayButtonSound(sfxIndex);
    }
}


