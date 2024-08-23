using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter_0Scenario : MonoBehaviour
{
    [SerializeField] GameObject inventory;
    //[SerializeField] QuestionDataBaseObject chapter0QuestionData;
    private void Start()
    {
        Manager.Chapter.chapter = 0;
        ChoiceForWork();
        inventory.SetActive(true);
        inventory.SetActive(false);
    }
    private void ChoiceForWork()
    {
        //if (Manager.Chapter.questionObject == null)
        //{
        //    Manager.Chapter.questionObject = chapter0QuestionData.Questions[0];
        //}
    }
}
