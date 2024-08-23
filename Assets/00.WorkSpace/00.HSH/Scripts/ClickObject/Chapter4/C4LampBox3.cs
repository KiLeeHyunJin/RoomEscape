using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C4LampBox3 : ClickObject
{
    [SerializeField] bool sizeChanged;

    private void Awake()
    {
        sizeChanged = false;
    }
    private void Start()
    {
        changeActiveValue = 1;
    }
    public override void ClickEffect()
    {
        switch(state)
        {
            case 0:
                break;
            case 1:
                PopUp();
                break;
            case 2:
                break;
        }
    }
    private void Update()
    {
        if (sizeChanged == false)
        {
            sizeChanged = true;
            gameObject.SetActive(false);
        }
    }
}
