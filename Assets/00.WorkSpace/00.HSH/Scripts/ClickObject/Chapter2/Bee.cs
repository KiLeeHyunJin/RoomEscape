using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bee : ClickObject
{
    [SerializeField] ClickObject _Farm;
    [SerializeField] RectTransform movePosition;
    public override void ClickEffect()
    {
        switch(state)
        {
            case 0:
                LogPlay(sentenceNum);
                break;
            case 1:
                GetItem(item);
                PopUp();
                state = 2;
                break;
            case 2:
                break;
        }
    }
    private void Update()
    {
        if (state == 2)
        {
            gameObject.GetComponent<RectTransform>().localPosition = movePosition.localPosition;
        }
    }
}
