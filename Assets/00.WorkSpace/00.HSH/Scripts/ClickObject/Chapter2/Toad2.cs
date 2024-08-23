using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Toad2 : ClickObject
{
    [SerializeField] SqueakHome squeakHome;
    [SerializeField] ScriptableItem _Key2;
    [SerializeField] bool isHammer = false;
    [SerializeField] bool isDoorknob = false;
    [SerializeField] bool allCheck = true;
    [SerializeField] int sentenceNum2;
    private void Start()
    {
        changeActiveValue = 1;
    }
    public override void ClickEffect()
    {
        switch(state)
        {
            case 0:
                break;
            case 1:
                Interact();
                SubInteract();
                if (isHammer == false || isDoorknob == false)
                {
                    Manager.Chapter.HintData.SetClearQuestion(10);
                    LogPlay(sentenceNum);
                }
                break;
        }
    }

    public override void InteractEffect()
    {
        //if(isDoorknob)
        //{
        //    state = 2;
        //    squeakHome.state = 1;
        //    LogPlay(sentenceNum);
        //}
        //else
        //{
        //    isHammer = true;
        //}

        isHammer = true;
    }

    public void SubInteract()
    {
        if (Manager.Inventory.UsingItemIndex == -1)
        {
            return;
        }
        string usingItem = Manager.Inventory.GetCurrentUsingItemName();
        if (_Key2 == null )
        {
            return;
        }
        if (!string.Equals(_Key2.name, usingItem))
        {
            Debug.Log($"00000 {usingItem}");
            return;
        }
        Manager.Inventory.OnSuccessUseItem();

        //if (isHammer)
        //{
        //    state = 2;
        //    squeakHome.state = 1;
        //    LogPlay(sentenceNum);
        //}
        //else
        //{
        //    isDoorknob = true;
        //}
        isDoorknob = true;
    }
    private void Update()
    {
        if (isDoorknob && isHammer && allCheck)
        {
            allCheck = false;
            state = 2;
            squeakHome.state = 2;
            LogPlay(sentenceNum2);
        }
    }

}