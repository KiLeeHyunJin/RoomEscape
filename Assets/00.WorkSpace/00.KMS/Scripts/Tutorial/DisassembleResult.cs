using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DisassembleResult : MonoBehaviour
{
    //private bool hasObtainedKeyParts = false;

    private void Start()
    {
        CheckForKeyParts();
        Manager.Text.TextChange();
    }

    void CheckForKeyParts() // 열쇠 조각 확인
    {
        Message.Log("열쇠 조각 확인");
        //int keyPartCount = GetKeyPartCount();
        //if (keyPartCount >= 1)
        //{
        //    hasObtainedKeyParts = true;
        //}
    }

    int GetKeyPartCount()
    {
        int count = 0;
        int idx = InventoryManager.Instance.FindItemIndex("Chapter0FinalKey");
        if (idx >= 0)
        {
            count = InventoryManager.Instance[idx].count;
        }
        return count;
    }
}
