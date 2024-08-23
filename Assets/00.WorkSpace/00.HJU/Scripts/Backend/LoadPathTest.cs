using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadPathTest : MonoBehaviour
{
    public string path;

    [ContextMenu("주소 불러오기")]
    public void a()
    {
        path = BackendChartData.LoadPath(1);
        Debug.Log(path);
    }
    
}
