using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C5Lope : ClickObject
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] GameObject beforeImage;
    [SerializeField] GameObject afterImage;
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    public override void ClickEffect()
    {
        switch(state)
        {
            case 0:
                PopUp();
                break;
            case 1:
                Interact();
                break;
                case 2:
                Manager.Game.GameClearPopup();
                break;
        }
    }
    public override void InteractEffect()
    {
        state = 2;
        StartCoroutine(LopeDown());
        Manager.Chapter.HintData.SetClearQuestion(6);
    }

    IEnumerator LopeDown()
    {
        float duration = 1.0f; // 애니메이션 지속 시간
        float elapsedTime = 0f;
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 endPos = new Vector2(startPos.x, startPos.y - 200);

        while (elapsedTime < duration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = endPos; // 최종 위치  설정
    }
    private void Update()
    {
        if (currentState == 0)
        {
            beforeImage.SetActive(true);
            afterImage.SetActive(false);
        }
        else
        {
            beforeImage.SetActive(false);
            afterImage.SetActive(true);
        }
    }
}
