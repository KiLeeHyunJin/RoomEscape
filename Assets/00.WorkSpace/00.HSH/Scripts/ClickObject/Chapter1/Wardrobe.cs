using UnityEngine;

public class Wardrobe : ClickObject
{
    [SerializeField] PopUpUI LockImage;
    [SerializeField] GameObject OpenImage;
    [SerializeField] bool check = true;
    [SerializeField] bool stateCheck = true;

    private void Start()
    {
        changeActiveValue = 2;
    }

    private void Update()
    {
        if (check)
        {
            check = false;
            OpenImage.SetActive(false);
        }

        if ( state == 2 && stateCheck)
        {
            stateCheck = false;
            OpenImage.SetActive(true);
            gameObject.SetActive(false);
        }
    }
    public override void ClickEffect()
    {
        switch (state)
        {
            case 0:
                Interact();
                if ( state == 0)
                {
                    Manager.UI.ShowPopUpUI(LockImage);
                }
                else
                {
                    return;
                }
                break;
            case 1:
                PopUp();
                break;
            case 2:
                break;
        }
    }

    public override void InteractEffect()
    {
        state = 1;
        PopUp();
    }
}
