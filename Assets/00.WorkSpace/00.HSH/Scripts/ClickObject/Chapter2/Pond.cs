using UnityEngine;

public class Pond : ClickObject
{
    public override void ClickEffect()
    {
        switch (state)
        {
            case 0:
                Interact();
                break;
            case 1:
                break;
        }
    }
    public override void InteractEffect()
    {
        state = 1;
        ChangeImage();
        GetItem(item);
        PopUp();
        Debug.Log("상호작용 됨");
    }
}
