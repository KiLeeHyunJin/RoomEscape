public class ChocoDoorNumber : ClickObject
{
    private void Start()
    {
        changeActiveValue = 1;
    }
    public override void ClickEffect()
    {
        switch (state)
        {
            case 0:
                Manager.Chapter._clickObject = this;
                Manager.UI.ShowPopUpUI(popUpUI);
                break;
            case 1:
                break;
        }
    }
}