using BackEnd;
using LitJson;
using System;
using TMPro;
using UnityEngine;

public class TESTLogChartController : MonoBehaviour
{
    [SerializeField] TMP_Text logNum;
    [SerializeField] TMP_Text logMessage;
    int index = 100;




    private void Awake()
    {
        // logNum.text = $"logNum : {BackendChartData.logChart[index].korean}";
        //logMessage.text = $"logMessage : {BackendChartData.logChart[index].english}";

        BackendChartData.logChart.TryGetValue(index, out LogChartData logChartData);
        logNum.text = $"korean : {logChartData.korean}";
        logMessage.text = $"english : {logChartData.english}";
    }

    public void NextLog()
    {
        index += 1;
        BackendChartData.logChart.TryGetValue(index, out LogChartData logChartData);
        logNum.text = $"korean : {logChartData.korean}";
        logMessage.text = $"english : {logChartData.english}";
    }

}
