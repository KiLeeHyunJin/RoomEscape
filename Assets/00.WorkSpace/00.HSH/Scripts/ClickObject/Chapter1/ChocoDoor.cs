using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChocoDoor : ClickObject
{
    [SerializeField] bool offCheck = true;
    [SerializeField] bool stateCheck=true;
    [SerializeField] GameObject door;
    [SerializeField] GameObject openDoor;
    [SerializeField] PopUpUI secondpopup;
     
    private void Update()
    {
        if (offCheck)
        {
            offCheck = false;
            openDoor.SetActive(false);
        }

        if (stateCheck && state==2)
        {
            stateCheck=false;
            door.SetActive(false);
            openDoor.SetActive(true);
        }
    }
    public override void ClickEffect()
    {
        switch (state)
        {
            case 0:
                Interact();
                if (state ==0)
                {
                    PopUp();//잠김 이미지
                }
                break;
            case 1:
                Manager.Chapter._clickObject = this;
                Manager.UI.ShowPopUpUI(secondpopup);
                break;
            case 2:
                Manager.Game.GameClearPopup();
                break;
        }
    }
    public override void InteractEffect()
    {
        state = 1;
        Manager.Chapter._clickObject = this;
        Manager.UI.ShowPopUpUI(secondpopup);
    }
}
