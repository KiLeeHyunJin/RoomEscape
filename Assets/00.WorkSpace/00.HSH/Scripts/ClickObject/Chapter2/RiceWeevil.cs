using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiceWeevil : ClickObject
{
    [SerializeField] GameObject watering;
    [SerializeField] GameObject storage;

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
                break;
        }
    }
    public override void InteractEffect()
    {
        PopUp();
        //GetItem(containItem);
        Debug.Log("상호작용 됨");
    }
    private void Update()
    {
        if (state == 2)
        {
            storage.SetActive(true);
            watering.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
