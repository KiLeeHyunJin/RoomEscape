using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChapterReadyUI : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI title;
    public void Init(Sprite _icon,int _titleId)
    {
        title.name = _titleId.ToString();
        icon.sprite = _icon;
    }
}
