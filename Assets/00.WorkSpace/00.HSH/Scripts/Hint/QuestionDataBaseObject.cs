using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Question Database", menuName = "Hint System/Questions/Question Database")]
public class QuestionDataBaseObject : ScriptableObject, ISerializationCallbackReceiver
{
    public int limitHintCount;
    public int limitHintQuestionIdx;
    public QuestionObject[] Questions;

    [ContextMenu("Update ID's")]
    public void UpdateQuestionID()
    {
        for (int i = 0; i < Questions.Length; i++)
        {
            if (Questions[i].questionId != i)
            {
                Questions[i].questionId = i;
            }
        }
    }
    public void OnBeforeSerialize()
    {
        UpdateQuestionID();
    }
    public void OnAfterDeserialize()
    {

    }
}
