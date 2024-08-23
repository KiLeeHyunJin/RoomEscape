
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SequentialImageMover : PopUpUI
{
    [System.Serializable]
    public class ImageMoveInfo
    {
        public RectTransform image; // 이동할 이미지의 RectTransform
        public bool moveLeft; // 왼쪽으로 이동할지 여부
    }

    [SerializeField] private ImageMoveInfo[] imageMoveInfos; // 이미지와 방향 정보를 담은 배열
    [SerializeField] private float moveDistance = 100f; // 이동할 거리
    [SerializeField] private float moveDuration = 0.5f; // 이동하는 데 걸리는 시간

    private Queue<ImageMoveInfo> imageQueue; // 이미지 순서를 관리할 큐

    private void Start()
    {
        // 이미지 정보를 큐에 추가
        imageQueue = new Queue<ImageMoveInfo>(imageMoveInfos);
        StartCoroutine(MoveImagesSequentially());
    }

    private IEnumerator MoveImagesSequentially()
    {
        //큐에서 이미지를 전부 꺼낼 때까지
        while (imageQueue.Count > 0)
        {
            ImageMoveInfo imageMoveInfo = imageQueue.Dequeue();
            RectTransform image = imageMoveInfo.image;
            Vector2 originalPosition = image.anchoredPosition;
            Vector2 direction = imageMoveInfo.moveLeft ? Vector2.left : Vector2.right;
            Vector2 targetPosition = originalPosition + direction * moveDistance;

            // 이동
            yield return StartCoroutine(MoveToPosition(image, targetPosition));

            // 원래 위치로 돌아옴
            yield return StartCoroutine(MoveToPosition(image, originalPosition));
        }
    }

    private IEnumerator MoveToPosition(RectTransform image, Vector2 targetPosition)
    {
        Vector2 startPosition = image.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            // 위치를 선형 보간하여 이동
            image.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 정확한 목표 위치로 설정
        image.anchoredPosition = targetPosition;
    }
}
