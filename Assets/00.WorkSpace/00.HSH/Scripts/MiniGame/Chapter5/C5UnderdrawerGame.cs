using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class C5UnderdrawerGame : SizeChangeGame
{
    private void Start()
    {
        // state 바꿀지, 아이템 줄지, state 몇으로 바꿀지 선택하는 부분
        // C++식?
        stateChangeCheck = true;
        giveItemCheck = true;
        questionClearCheck = true;
        changeState = 1;
        questionNum = 1;
    }
    public override void GameEffect()
    {
        base.GameEffect();
        Manager.Chapter.HintData.SetClearQuestion(questionNum);
    }
}
