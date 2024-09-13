using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class StartBlink : MonoBehaviour
{
    TMP_Text text;

    void Start()
    {
        text = this.GetOrAddComponent<TMP_Text>();
        StartCoroutine(FadeTextToFullAlpha());
    }

    public IEnumerator FadeTextToFullAlpha() // 알파값 0에서 1로 전환
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        bool isUp = true;
        while (true)
        {
            if (isUp)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (Time.deltaTime / 1f));
                if (text.color.a >= 1.0f)
                    isUp = false;
            }
            else
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime / 1f));
                if (text.color.a <= 0f)
                    isUp = true;
            }
            yield return null;
        }
    }
}
