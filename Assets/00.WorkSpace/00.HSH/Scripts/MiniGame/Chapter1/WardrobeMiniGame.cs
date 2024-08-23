using UnityEngine;

public class WardrobeMiniGame : LockKeyGame
{
    [SerializeField] GameObject ClearButton;
    public new void CheckAnswer()
    {
        for (int i = 0; i < answers.Length; i++)
        {
            if (answers[i] != texts[i].text)
            {
                return;
            }
        }

        if (Manager.Chapter._clickObject.item != null)
        {
            Manager.Chapter._clickObject?.GetItem(Manager.Chapter._clickObject.item);
        }

        if (clearMessage != null)
        {
            clearMessage.SetActive(true);
        }
        Manager.Chapter._clickObject.state = 2;
        Manager.Chapter._clickObject.ChangeImage();
        ClearButton.SetActive(true);
    }
    public void Change2()
    {
        Manager.Chapter._clickObject.state = 2;
        Close();
    }
}
