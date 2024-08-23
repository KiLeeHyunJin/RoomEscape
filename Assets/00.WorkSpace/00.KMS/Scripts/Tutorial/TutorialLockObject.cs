using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLockObject : ClickObject
{
    private bool hasItem;

    void CheckForKey() // Chapter0FinalKey이 있는지 체크
    {
        int itemCount = CheckItem();
        if (itemCount >= 1)
        {
            hasItem = true;
        }
    }

    int CheckItem()
    {
        int count = 0;
        int idx = InventoryManager.Instance.FindItemIndex("Chapter0FinalKey"); // 인벤토리에 해당 아이템이 있는지 찾기
        if (idx >= 0)
        {
            count = InventoryManager.Instance[idx].count;
        }
        return count;
    }

    public override void ClickEffect()
    {
        Manager.Text.TextChange();
        CheckForKey();
        if (hasItem) // Chapter0FinalKey을 보유 했을 경우 자물쇠 제거
        {
            switch (state)
            {
                case 0:
                    // PopUp();
                    gameObject.SetActive(false);
                    break;
                case 1:
                    break;
            }
        }
        else
        {
            PopUp(); // Chapter0FinalKey이 없을 경우 팝업뜸
        }
    }
}
