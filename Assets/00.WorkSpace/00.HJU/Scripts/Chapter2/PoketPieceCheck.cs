using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoketPieceCheck : PopUpUI
{
    [SerializeField] GameObject poketPiecePos;
    [SerializeField] GameObject clearMessage;

    public bool IsClear()
    {
        for (int i = 0; i < poketPiecePos.transform.childCount; i++)
        {
            if (poketPiecePos.transform.GetChild(i).childCount == 0)
            {
                return false;
            }
        }
        Debug.Log("clear");
        Manager.Chapter._clickObject.GetItem(Manager.Chapter._clickObject.item);
        Manager.Chapter._clickObject.state = 3;
        clearMessage.SetActive(true);
        return true;
    }
}
