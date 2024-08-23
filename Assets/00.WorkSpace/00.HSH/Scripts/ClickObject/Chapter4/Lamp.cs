using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lamp : ClickObject
{
    [SerializeField] ClickObject lampBox1;
    [SerializeField] ClickObject lampBox2;
    [SerializeField] ClickObject lampBox3;
    [SerializeField] Button button;

    private void Start()
    {
        changeActiveValue = 2;
        changeImageValue = 1;
        button = GetComponent<Button>();
    }
    public override void ClickEffect()
    {
        switch(state)
        {
            case 0:
                break;
            case 1:
                lampBox1.state = 1;
                lampBox2.state = 1;
                lampBox3.state = 1;
                state = 2;
                button.enabled = false;
                break;
            case 2:
                break;
        }
    }
}
