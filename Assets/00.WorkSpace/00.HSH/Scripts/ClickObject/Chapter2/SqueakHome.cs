using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SqueakHome : ClickObject
{
    [SerializeField] ClickObject _Toad2;
    private void Start()
    {
        changeImageValue = 2;
    }
    public override void ClickEffect()
    {
        switch(state)
        {
            case 0:
                Interact();
                break;
            case 1:
                break;
            case 2:
                PopUp();
                break;
        }
    }
    public override void InteractEffect()
    {
        state = 1;
        //_Toad2.gameObject.SetActive(true);
        _Toad2.state = 1;
    }

    private void Update()
    {
        if (state == 0)
        {
            _Toad2.gameObject.SetActive(false);
        }
        else
        {
            _Toad2.gameObject.SetActive(true);
        }
    }
}
