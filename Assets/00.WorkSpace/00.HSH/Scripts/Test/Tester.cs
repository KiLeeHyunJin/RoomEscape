using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    private void Start()
    {
        Debug.Log($"kor : {Manager.Data.UserGameData.kor}");
    }
}
