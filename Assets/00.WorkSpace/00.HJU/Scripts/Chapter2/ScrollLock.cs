using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollLock : MonoBehaviour, IEndDragHandler, IPointerDownHandler
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform viewPortTransform;
    [SerializeField] private RectTransform contentTransform;
    [SerializeField] private VerticalLayoutGroup vlg;
    [SerializeField] private RectTransform[] cellList;

    [SerializeField] LockManager lockManager;
    private Vector2 oldVelocity;
    private bool isUpdated;
    private float cellHeightWithSpacing;
    public int index; 

    private void Start()
    {
        oldVelocity = Vector2.zero;
        isUpdated = false;
        cellHeightWithSpacing = cellList[0].rect.height + vlg.spacing;

        int cellsToAdd = Mathf.CeilToInt(viewPortTransform.rect.height / cellHeightWithSpacing);

        for (int i = 0; i < cellsToAdd; i++)
        {
            RectTransform rt = Instantiate(cellList[i % cellList.Length], contentTransform);
            rt.SetAsLastSibling();
        }

        for (int i = 0; i < cellsToAdd; i++)
        {
            int num = (cellList.Length - i - 1 + cellList.Length) % cellList.Length;
            RectTransform rt = Instantiate(cellList[num], contentTransform);
            rt.SetAsFirstSibling();
        }

        CenterOnFirstCell(cellsToAdd);
    }

    private void Update()
    {
        if (isUpdated)
        {
            isUpdated = false;
            scrollRect.velocity = oldVelocity;
        }

        if (contentTransform.localPosition.y < 0 || contentTransform.localPosition.y > cellList.Length * cellHeightWithSpacing)
        {
            AdjustContentPosition();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isUpdated)
        {
            isUpdated = false;
            scrollRect.velocity = oldVelocity;
        }

        AdjustContentPosition();
        StartCoroutine(CenterClosestCell());
        scrollRect.velocity = Vector2.zero;

        if (lockManager != null)
        {
            lockManager.CheckCombination();
        }
    }

    private void AdjustContentPosition()
    {
        if (contentTransform.localPosition.y < 0)
        {
            oldVelocity = scrollRect.velocity;
            contentTransform.localPosition += new Vector3(0, cellList.Length * cellHeightWithSpacing, 0);
            isUpdated = true;
        }
        else if (contentTransform.localPosition.y > cellList.Length * cellHeightWithSpacing)
        {
            oldVelocity = scrollRect.velocity;
            contentTransform.localPosition -= new Vector3(0, cellList.Length * cellHeightWithSpacing, 0);
            isUpdated = true;
        }
    }

    private void CenterOnFirstCell(int cellsToAdd)
    {
        contentTransform.localPosition = new Vector3(
            contentTransform.localPosition.x,
            (cellsToAdd * cellHeightWithSpacing) - (viewPortTransform.rect.height / 2) + (cellHeightWithSpacing / 2) - (vlg.spacing/2),
            contentTransform.localPosition.z
        );
    }

    private IEnumerator CenterClosestCell()
    {
        float minDistance = float.MaxValue;
        RectTransform closestCell = null;
        float viewportCenterY = viewPortTransform.localPosition.y - viewPortTransform.rect.height ;
        float contentOffsetY = contentTransform.localPosition.y;

        foreach (RectTransform cell in contentTransform)
        {
            float cellCenterY = cell.localPosition.y + contentOffsetY;
            float distance = Mathf.Abs(cellCenterY - viewportCenterY);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestCell = cell;
            }
        }

        if (closestCell != null)
        {
            float adjustment = (closestCell.localPosition.y + contentOffsetY) - viewportCenterY;
            Vector3 targetPosition = contentTransform.localPosition - new Vector3(0, adjustment, 0);
            Vector3 startPosition = contentTransform.localPosition;
            float elapsedTime = 0f;
            float duration = 0.3f; // Adjust the duration as needed

            while (elapsedTime < duration)
            {
                contentTransform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            contentTransform.localPosition = targetPosition;
        }
        Manager.Sound.PlayButtonSound(14);
    }
    public int GetCurrentNumber()
    {
        float minDistance = float.MaxValue;
        RectTransform closestCell = null;
        float viewportCenterY = viewPortTransform.localPosition.y - viewPortTransform.rect.height;
        float contentOffsetY = contentTransform.localPosition.y;

        foreach (RectTransform cell in contentTransform)
        {
            float cellCenterY = cell.localPosition.y + contentOffsetY;
            float distance = Mathf.Abs(cellCenterY - viewportCenterY);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestCell = cell;
            }
        }

        if (closestCell != null)
        {
            ScrollLockCell cellComponent = closestCell.GetComponent<ScrollLockCell>();
            if (cellComponent != null)
            {
                return cellComponent.number;
            }
        }

        return -1; // Invalid value if something goes wrong
    }

    public bool IsCorrectNumber()
    {
        return GetCurrentNumber() == lockManager.CorrectNumber[index];
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Manager.Sound.PlayButtonSound(14);
    }
}