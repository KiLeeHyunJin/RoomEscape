using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squeak : ClickObject
{
    [SerializeField] ClickObject _Toad;
    [SerializeField] ClickObject _MouseTrap;
    public override void ClickEffect()
    {
        switch(state)
        {
            case 0:
                LogPlay(sentenceNum);
                _Toad.state = 1;
                _MouseTrap.state = 1;
                break;
            case 1:
                break;
        }
    }
}