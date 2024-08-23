using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C4LampBox2 : ClickObject
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
