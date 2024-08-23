using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderdrawerGame : RhythmGame
{
    public override void GameEffect()
    {
        Manager.Chapter._clickObject.state = 1;
        Manager.Chapter._clickObject.GetItem(Manager.Chapter._clickObject.item);
    }
}
