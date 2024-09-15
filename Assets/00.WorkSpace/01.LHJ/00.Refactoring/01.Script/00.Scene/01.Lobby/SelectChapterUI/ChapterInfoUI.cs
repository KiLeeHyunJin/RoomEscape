using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChapterInfoUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] TextMeshProUGUI description;
    public void Init(int titleId, int infoId)
    {
        title.name = titleId.ToString();
        description.name = infoId.ToString();
    }
}
