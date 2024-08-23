using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiceweevilGame : RhythmGame
{
    public override void GameEffect()
    {
        Manager.Chapter._clickObject.state = 2;
        Manager.Chapter._clickObject.ChangeImage();
    }
}