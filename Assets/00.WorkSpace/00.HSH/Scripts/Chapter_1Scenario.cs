using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter_1Scenario : MonoBehaviour
{
    [SerializeField] GameObject Inven;
    [SerializeField] ScriptableItem i1;
    [SerializeField] ScriptableItem i2;
    [SerializeField] ScriptableItem i3;
    [SerializeField] ClickObject clickObject;

    void Start()
    {
        //Inven.SetActive(true);
        //Inven.SetActive(false);
        Manager.Chapter._clickObject = clickObject;
        //Manager.Chapter._clickObject.GetItem(i1);
        //Manager.Chapter._clickObject.GetItem(i2);
        //Manager.Chapter._clickObject.GetItem(i3);
    }
}
