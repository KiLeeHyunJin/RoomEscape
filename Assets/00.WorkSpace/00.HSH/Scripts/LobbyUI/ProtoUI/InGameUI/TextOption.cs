using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UserGameData;

public class TextOption : MonoBehaviour
{
    [SerializeField] GameObject _korean;
    [SerializeField] GameObject _english;
    private void Start()
    {
        if (Manager.Text._Iskr == true)
        {
            _korean.SetActive(true);
            _english.SetActive(false);
        }
        else
        {
            _korean.SetActive(false);
            _english.SetActive(true);
        }
    }

    private void OnDisable()
    {
        Manager.Data.UserGameData.SetData(GameDataEnum.Kor, Manager.Text._Iskr);
    }

    public void TextChange()
    {
        //ResolutionChanger();
        Manager.Text._Iskr = !Manager.Text._Iskr;
    }
    private void ResolutionChanger()
    {
        TextMeshProUGUI[] childs = FindObjectsOfType<TextMeshProUGUI>();
        for (int i = 0; i < childs.Length; i++)
        {
            if (childs[i].TryGetComponent(out TextMeshProUGUI textMeshPro))
            {
                //오토 사이즈 설정
                textMeshPro.enableAutoSizing = true;
                //오토 사이즈 폰트 범위 설정
                textMeshPro.fontSizeMin = 1;
                textMeshPro.fontSizeMax = 999;
                //오프셋 범위 0으로 설정
                textMeshPro.rectTransform.offsetMax = textMeshPro.rectTransform.offsetMin = Vector2.zero;
                //한 Tick 이후 오토 사이즈 해제
                StartCoroutine(ChangeFont(textMeshPro));
            }
        }
    }
    IEnumerator ChangeFont(TextMeshProUGUI textMeshPro)
    {
        yield return null;
        textMeshPro.enableAutoSizing = false;
    }
}
