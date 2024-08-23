using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TestSceneData : MonoBehaviour
{
    public TestScene testSceneToSave; // 저장할 TestScene ScriptableObject 참조
    public string saveFilePath; // JSON 파일 경로 및 이름 설정 (예: "Assets/Resources/testScene.json")

    public void SaveScene()
    {
        string jsonData = JsonUtility.ToJson(testSceneToSave); // ScriptableObject를 JSON 문자열로 변환

        try
        {
            File.WriteAllText(saveFilePath, jsonData); // JSON 문자열을 파일에 씀
            Debug.Log("TestScene 저장 성공!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("TestScene 저장 오류: " + e.Message);
        }
    }
}