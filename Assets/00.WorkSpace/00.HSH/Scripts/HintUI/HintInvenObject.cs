using UnityEngine;

[CreateAssetMenu(fileName = "New Hint Inventory", menuName = "Hint System/Hint Inventory")]
public class HintInvenObject : ScriptableObject
{
    public QuestionDataBaseObject database;
    public HintInventory Viewer;
}


[System.Serializable]
public class HintInventory
{
    public HintSlot[] hintslots;

    //public void Clear()
    //{
    //    for (int i = 0; i < hintslots.Length; i++)
    //    {
    //        hintslots[i].Remove();
    //    }
    //}
}
[System.Serializable]
public class HintSlot
{
    [System.NonSerialized]
    int questionIdx;
    int hintIdx;
    public HintWindow parent;
    public Hint hint;
    //public HintObject hintObject
    //{
    //    get
    //    {
    //        if (hint.hintId >= 0)
    //        {
    //            return parent.QuestionDataBase.Questions[parent.Question.questionId].hints[hint.hintId];
    //        }
    //        return null;
    //    }
    //    set
    //    {

    //    }
    //}
    //public HintSlot()
    //{
    //    hint = new Hint();
    //}
    //public void UpdateSlot(Hint _hintSet)
    //{
    //    hint = _hintSet;
    //}
    //public void Remove()
    //{
    //    hint = new Hint();
    //}
}