using UnityEngine;
using UnityEngine.EventSystems;

public class HoleTargetImage : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] RectTransform targetRectTransform;
    [SerializeField] RectTransform holeRectTransform; // 구멍 이미지의 RectTransform
    [SerializeField] bool isDragging = false;
    private Vector2 lastMousePosition;

    private void OnEnable()
    {
        Manager.Sound.PlayButtonSound(23);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            Vector2 currentMousePosition = eventData.position;
            Vector2 delta = currentMousePosition - lastMousePosition;

            Vector2 newPosition = targetRectTransform.anchoredPosition + delta / CanvasScaleFactor();
            lastMousePosition = currentMousePosition;

            targetRectTransform.anchoredPosition = ClampToHoleBounds(newPosition);
        }
    }

    private float CanvasScaleFactor()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            return canvas.scaleFactor;
        }
        return 1.0f;
    }

    private Vector2 ClampToHoleBounds(Vector2 position)
    {
        // 구멍 이미지의 왼쪽 변 위치를 기준으로 배경 이미지의 이동을 제한
        float leftLimit = (targetRectTransform.sizeDelta.x * 0.5f) - (holeRectTransform.sizeDelta.x * 0.5f);
        float rightLimit = (targetRectTransform.sizeDelta.x * -0.5f) - (holeRectTransform.sizeDelta.x * -0.5f);
        float bottomLimit = (targetRectTransform.sizeDelta.y * 0.5f) - (holeRectTransform.sizeDelta.y * 0.5f);
        float topLimit = (targetRectTransform.sizeDelta.y * -0.5f) - (holeRectTransform.sizeDelta.y * -0.5f);
        

        if (position.x > leftLimit)
        {
            position.x = leftLimit;
        }

        if (position.y > bottomLimit)
        {
            position.y = bottomLimit;
        }

        if (position.x < rightLimit)
        {
            position.x = rightLimit;
        }

        if (position.y < topLimit)
        {
            position.y = topLimit;
        }


        return position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        lastMousePosition = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }
}
