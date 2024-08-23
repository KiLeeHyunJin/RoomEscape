using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;

public class ChapterUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI chapterName;

    public void Choice(string name)
    {
        chapterName.text = name;
    }
}
