using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseBoxPuzzle : SlidingPuzzle
{
    public override void GameEffect()
    {
        Manager.Chapter._clickObject.GetItem(Manager.Chapter._clickObject.item);
        Manager.Chapter._clickObject.state = 1;
    }
}
