using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C4LampBox1 : ClickObject
{
    // 바꿀 이미지가 있는데 리소스가 없음 받는데로 넣을것
    [SerializeField] bool sizeChanged;

    private void Start()
    {
        changeActiveValue = 1;
    }
    private void Awake()
    {
        sizeChanged = false;
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
