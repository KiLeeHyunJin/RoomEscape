using BackEnd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackendDebugManger : MonoBehaviour
{
    private void Awake()
    {
        var bro = Backend.BMember.GuestLogin();
        
    }

    private void Start()
    {
        BackendChartData.LoadAllChart();
    }
}

