public class C5UnderDrawer : ClickObject
{
    public ClickObject Wearer;
    private void Start()
    {

    }
    public override void ClickEffect()
    {
        switch (state)
        {
            case 0:
                PopUp();
                break;
            case 1:
                break;
        }
    }
}
