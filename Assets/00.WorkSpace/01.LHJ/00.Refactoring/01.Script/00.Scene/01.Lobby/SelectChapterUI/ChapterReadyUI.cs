using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChapterReadyUI : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI title;
    public void Init(Sprite icon,int titleId)
    {
        this.title.name = titleId.ToString();
        this.icon.sprite = icon;
    }
}
