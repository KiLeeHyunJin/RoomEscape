using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toad : ClickObject
{
    private void Start()
    {
        changeActiveValue = 2;
    }
    public override void ClickEffect()
    {
        switch(state)
        {
            case 0:
                break;
            case 1:
                // 저장할때 어쩔지가 문제
                LogPlay(sentenceNum);
                GetItem(item);
                break;
            case 2:
                break;
        }
    }
    public override void LogEffect()
    {
        PopUp();
        Debug.Log("여기 실행됨");
        state = 2;       
    }
    public void OnDisable()
    {
        //PopUp();
    }
}
