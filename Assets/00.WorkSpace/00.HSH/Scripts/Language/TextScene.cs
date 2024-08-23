using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextScene : MonoBehaviour
{
    public TextMeshProUGUI[] textMeshPros;
    public List<Dictionary<string, object>> textCSV;
    public bool korean;
    private void Start()
    {
        korean = true;
        LoadLanguage();
    }

    public void Korean()
    {
        textCSV = CSVReader.Read("LanguageData");
        for (int i = 0; i < textMeshPros.Length; i++)
        {
            textMeshPros[i].text = textCSV[i]["Korean"].ToString();
        }
    }
    public void English()
    {
        textCSV = CSVReader.Read("LanguageData");
        for (int i = 0; i < textMeshPros.Length; i++)
        {
            textMeshPros[i].text = textCSV[i]["English"].ToString();
        }
    }
    public void LoadLanguage()
    {
        if ( korean ==  true )
        {
            Korean();
        }
        else
        {
            English();
        }
    }
    public void ChangeLanguage()
    {
        if( korean == true)
        {
            korean = false;
            LoadLanguage();
        }
        else
        {
            korean = true;
            LoadLanguage();
        }
    }
}
