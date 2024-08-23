using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyTable : ClickObject
{
    [SerializeField] ScriptableItem tablecloth_Key;
    [SerializeField] ScriptableItem candlestick_Key;
    [SerializeField] ScriptableItem tableware_Key;

    [SerializeField] GameObject tablecloth_image;
    [SerializeField] GameObject candlestick_image;
    [SerializeField] GameObject tableware_image;

    [SerializeField] bool tablecloth = false;
    [SerializeField] bool candlestick = false;
    [SerializeField] bool tableware = false;

    [SerializeField] bool initialized = false;

    public override void ClickEffect()
    {
        switch (state)
        {
            case 0:
                TableInteract();
                break;
            case 1:
                Manager.Game.GameClearPopup();
                break;
        }
    }

    public void TableInteract()
    {
        if (Manager.Inventory.UsingItemIndex == -1)
        {
            return;
        }

        string usingItem = Manager.Inventory.GetCurrentUsingItemName();

        if (usingItem == null)
        {
            return;
        }

        bool itemUsed = false;

        if (tablecloth_Key != null && string.Equals(tablecloth_Key.name, usingItem))
        {
            tablecloth = true;
            tablecloth_image.SetActive(true);
            itemUsed = true;
        }
        else if (candlestick_Key != null && string.Equals(candlestick_Key.name, usingItem))
        {
            candlestick = true;
            candlestick_image.SetActive(true);
            itemUsed = true;
        }
        else if (tableware_Key != null && string.Equals(tableware_Key.name, usingItem))
        {
            tableware = true;
            tableware_image.SetActive(true);
            itemUsed = true;
        }

        if (itemUsed)
        {
            Manager.Inventory.OnSuccessUseItem();
            Debug.Log($"아이템 사용 성공: {usingItem}");
        }
        else
        {
            Debug.Log($"아이템 사용 실패: {usingItem}");
        }
    }

    private void Update()
    {
        if (!initialized)
        {
            tablecloth_image.SetActive(false);
            candlestick_image.SetActive(false);
            tableware_image.SetActive(false);
            initialized = true;
        }

        if (tablecloth && candlestick && tableware)
        {
            Manager.Chapter.HintData.SetClearQuestion(12);
            state = 1;
        }
    }
}
