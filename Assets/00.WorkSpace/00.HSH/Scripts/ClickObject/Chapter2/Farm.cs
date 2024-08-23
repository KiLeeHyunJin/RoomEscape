using UnityEngine;

public class Farm : ClickObject
{
    [SerializeField] GameObject flower;
    [SerializeField] ClickObject _Bee;
    
    public override void ClickEffect()
    {
        switch (state)
        {
            case 0:
                Interact();
                break;
        }
    }
    public override void InteractEffect()
    {
        Manager.Chapter.HintData.SetClearQuestion(4);
        state = 1;
        _Bee.state = 1;
    }
    private void Update()
    {
        if ( state == 0)
        {
            flower.SetActive(false);
        }
        else if ( state >= 1)
        {
            flower.SetActive(true);
        }
    }
}
