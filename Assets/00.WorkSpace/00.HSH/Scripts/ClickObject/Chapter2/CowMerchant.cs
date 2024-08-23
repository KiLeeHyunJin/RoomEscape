using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;

public class CowMerchant : ClickObject
{
    [SerializeField] bool _gameEnd;
    [SerializeField] int _sentenceNum2;
    [SerializeField] ScriptableItem _tableware;
    [SerializeField] ClickObject _Crow;
    [SerializeField] PopUpUI silverWareget;

    [SerializeField] ScriptableItem i1;
    [SerializeField] ScriptableItem i2;
    [SerializeField] ScriptableItem i3;

    private void Start()
    {
        //GetItem(i1);
        //GetItem(i2);
        //GetItem(i3);
    }

    public override void LogEffect()
    {
        if (state ==2)
        {
            PopUp();
        }
    }

    public override void ClickEffect()
    {
        switch (state)
        {
            case 0:
                Interact();
                if( state == 0)
                {
                    LogPlay(sentenceNum);
                }
                break;
            case 1:
                LogPlay(_sentenceNum2);
                state = 2;
                _Crow.state = 1;
                break;
            case 2:
                PopUp();
                break;
            case 3:
                Interact();
                if (state == 3)
                {
                    LogPlay(sentenceNum);
                }
                break;
            case 4:
                break;
        }
    }
    public override void InteractEffect()
    {
        state = 4;
        GetItem(_tableware);
        Manager.UI.ShowPopUpUI(silverWareget);
        ChangeImage();
        Debug.Log("상호작용 됨");
    }
}
