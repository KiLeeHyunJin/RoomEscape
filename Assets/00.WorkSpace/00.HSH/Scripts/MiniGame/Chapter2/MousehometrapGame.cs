using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousehometrapGame : RhythmGame
{
    public override void GameEffect()
    {
        Manager.Chapter._clickObject.state = changeState;
    }
}
