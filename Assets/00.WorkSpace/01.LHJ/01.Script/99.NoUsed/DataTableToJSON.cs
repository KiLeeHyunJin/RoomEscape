using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
[Serializable]
public struct DataList
{
    public string name;
    public TypeCode type;
    public string data;
}
[Serializable]
public struct SettingData
{
    public DataList[] type;
}
public class DataTableToJSON : MonoBehaviour
{
    #if UNITY_EDITOR
        private string path => Path.Combine(Application.dataPath, $"Resources/Data");
    #else
        private string path => Path.Combine(Application.persistentDataPath, $"Resources/Data/SaveLoad");
    #endif
    [SerializeField] SettingData data;

    [ContextMenu("ApplyMakeJoson")]
    public void MakeToJson()
    {
        if (Directory.Exists(path) == false)
        {
            Directory.CreateDirectory(path);
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText($"{path}/data.txt", json);
    }
}
