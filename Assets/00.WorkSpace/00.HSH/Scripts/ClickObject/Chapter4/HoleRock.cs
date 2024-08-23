using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleRock : ClickObject
{
    [SerializeField] ClickObject shovel;
    private void Start()
    {
        changeActiveValue = 2;
    }
    public override void ClickEffect()
    {
        switch(state)
        {
            case 0:
                PopUp();
                break;
            case 1:
                break;
        }
    }
    private void Update()
    {
        if (state == 0)
        {
            shovel.gameObject.SetActive(false);
        }
        else if (state == 1)
        {
            shovel.gameObject.SetActive(true);
            state = 2;
        }
        else
        {
            return;
        }
    }
}
