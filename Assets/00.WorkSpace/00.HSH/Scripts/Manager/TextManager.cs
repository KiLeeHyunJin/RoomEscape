using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
public class TextManager : Singleton<TextManager>
{
    private bool _isKr;// 한영체크
    public bool _Iskr
    {
        get { return _isKr; }
        set
        {
            if (_isKr != value)
            {
                _isKr = value;
                TextChange();
            }
        }
    }
    public int _SentenceNum; // 재생할 회화 번호
    [SerializeField] PopUpUI _logUI;

    string madetext;
    private void Start()
    {
        _isKr = true;
    }


    public void SetScene(int i)
    {
        _SentenceNum = i;
        Manager.UI.ShowPopUpUI(_logUI);
    }
    public void TextChange()
    {
        TextMeshProUGUI[] allText; // 씬에 존재하는 모든 텍스트 매쉬
        allText = FindObjectsOfType<TextMeshProUGUI>();

        // 배열에 저장된 모든 TextMeshProUGUI 돌아가면서 한영체크
        for (int i = 0; i < allText.Length; i++)
        {
            if (int.TryParse(allText[i].name, out _))
            {
                BackendChartData.logChart.TryGetValue(int.Parse(allText[i].name), out LogChartData logChartData);
                if ( logChartData != null)
                {
                    if (Manager.Text._isKr == true)
                    {       
                        allText[i].text = logChartData.korean;
                    }
                    else
                    {
                        allText[i].text = logChartData.english;
                    }
                }
            }
            else
            {
                continue;
            }
        }
    }
    public string TextMake(int i)
    {
        BackendChartData.logChart.TryGetValue(i, out LogChartData logChartData);
        if (logChartData != null)
        {
            if (Manager.Text._isKr == true)
            {
                madetext = logChartData.korean;
            }
            else
            {
                madetext = logChartData.english;
            }
        }
        return madetext;
    }
}