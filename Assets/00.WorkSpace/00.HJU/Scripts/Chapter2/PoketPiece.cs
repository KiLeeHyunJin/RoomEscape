using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PoketPiece : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [SerializeField] PoketPieceCheck poketPieceCheck;
    [SerializeField] GameObject poketPiecePos;
    [SerializeField] GameObject poketPieceGroub;
    [SerializeField] float snapOffset;

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Vector3.Distance(poketPiecePos.transform.position, transform.position) < snapOffset)
        {
            transform.SetParent(poketPiecePos.transform);
            transform.localPosition = Vector3.zero;
        }
        else
        {
            transform.SetParent(poketPieceGroub.transform);
        }

        if (poketPieceCheck.IsClear())
        {
            Debug.Log("Clear");
        }
        else
        {
            Debug.Log("Fail");
        }
    }
}
