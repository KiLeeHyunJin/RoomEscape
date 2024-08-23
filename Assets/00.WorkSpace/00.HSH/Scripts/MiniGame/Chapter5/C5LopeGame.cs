using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C5LopeGame : MoveMiniGame
{
    private void Start()
    {
        //인스펙터
        //changeState = 1;
        //questionNum = 0;
    }
    public override void GameEffect()
    {
        Manager.Chapter._clickObject.state = changeState;
        Manager.Sound.PlayButtonSound(6);
        Manager.Chapter.HintData.SetClearQuestion(questionNum);
    }
}
