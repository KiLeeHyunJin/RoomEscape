using System.Diagnostics;
using Unity.VisualScripting;

public class Safe : ClickObject
{
    public override void ClickEffect()
    {
        //Message.Log(state);
        switch (state)
        {
            case 0:
                Manager.Text.TextChange();
                PopUp();
                break;
            case 1:
                gameObject.SetActive(false);
                break;
        }
    }
}
