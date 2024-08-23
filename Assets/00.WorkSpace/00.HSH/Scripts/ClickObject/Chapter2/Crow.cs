using UnityEngine;

public class Crow : ClickObject
{
    [SerializeField] ClickObject _CowMerchant;
    public override void ClickEffect()
    {
        switch (state)
        {
            case 0:
                LogPlay(sentenceNum);
                Debug.Log($"QuestionCount : {Manager.Chapter.HintData.GetQuestionCount()}");
                Manager.Chapter.HintData.SetClearQuestion(0);
                _CowMerchant.state = 1;
                break;
            case 1:
                Interact();
                if(state == 1)
                {
                    LogPlay(sentenceNum);
                }
                break;
            case 2:
                break;
        }
    }
    public override void InteractEffect()
    {
        GetItem(item);
        Manager.Chapter.HintData.SetClearQuestion(7);
        PopUp();
        state = 2;
    }
}
