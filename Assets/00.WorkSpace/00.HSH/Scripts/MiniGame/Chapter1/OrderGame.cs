using System.Text;
using UnityEngine;

public class OrderGame : MonoBehaviour
{
    [SerializeField] int ChoiceCost;

    [Header("Answer")]
    [SerializeField] string Answer;
    [SerializeField] string Submission;

    public void InputSubmission(string some)
    {
        if (ChoiceCost > 0)
        {
            ChoiceCost--;
            StringPlus(some);
            if (ChoiceCost == 0)
            {
                CheckAnswer();
            }
            return;
        }
        //else if (ChoiceCost == 0)
        //{
        //    ChoiceCost--;
        //    StringPlus(some);
        //    CheckAnswer();
        //    return;
        //}

    }
    public void StringPlus(string some)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(some);
        Submission = Submission + sb.ToString();
    }
    public void CheckAnswer()
    {
        if (Answer == Submission)
        {
            Submission = "";
            ChoiceCost = 4;
            Debug.Log("Success");
        }
        else
        {
            Submission = "";
            ChoiceCost = 4;
            Debug.Log("Fail");
        }
    }

}
