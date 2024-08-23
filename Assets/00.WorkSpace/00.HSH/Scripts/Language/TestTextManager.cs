using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class TestTextManager : MonoBehaviour
{
    [SerializeField] TestTextDatabase textDatabase;
    public List<Dictionary<string, object>> textCSV;

    private string Language;
    private void Start()
    {
        textCSV = CSVReader.Read("LanguageData");
        LoadFromCSV();
        SaveToJson();
        LoadFromJson();
    }
    public void SaveToJson()
    {
        Debug.Log("인벤토리 저장 (JSON)");

        //오프라인
        // 1. JSON 문자열 준비
        string jsonData = JsonUtility.ToJson(new textData(textDatabase), true); // 개인 필드 포함

        // 2. 저장 경로 가져오기
        string savePath = Path.Combine(Application.persistentDataPath, "text.json"); // 경로

        // 3. JSON 데이터를 파일에 쓰기
        try
        {
            using (FileStream fileStream = File.Create(savePath))
            {
                byte[] data = Encoding.UTF8.GetBytes(jsonData);
                fileStream.Write(data, 0, data.Length);
            }
            Debug.Log("인벤토리 JSON으로 성공적으로 저장됨!");
        }
        catch (Exception e)
        {
            Debug.LogError("인벤토리를 JSON으로 저장하는 중 오류 발생: " + e.Message);
        }

    }

    [ContextMenu("로드 (JSON)")]
    public void LoadFromJson()
    {
        Debug.Log("인벤토리 로드 (JSON)");
        // 오프라인
        // 1. 저장 경로 가져오기
        string savePath = Path.Combine(Application.persistentDataPath, "text.json"); // 경로

        // 2. 파일 존재 여부 확인
        if (File.Exists(savePath))
        {
            // 3. JSON 데이터를 파일에 읽기
            try
            {
                using (FileStream fileStream = File.OpenRead(savePath))
                {
                    byte[] data = new byte[(int)fileStream.Length];
                    fileStream.Read(data, 0, data.Length);

                    string jsonData = Encoding.UTF8.GetString(data);

                    // 4. JSON 데이터를 Container 객체로 역직렬화
                    textData newContainer = JsonUtility.FromJson<textData>(jsonData);

                    for (int i = 0; i < textDatabase.testTexts.Length; i++) // 배열마다 분배
                    {
                        textDatabase.testTexts[i].data = newContainer.testTs[i];
                    }
                    Debug.Log("인벤토리 JSON에서 성공적으로 로드됨!");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("JSON에서 인벤토리를 로드하는 중 오류 발생: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("인벤토리 JSON 파일을 찾을 수 없음: " + savePath);
        }
    }
    private void LoadFromCSV()
    {
        Debug.Log(textCSV[0]["Korean"]);
        for (int i = 0; i < textCSV.Count; i++)
        {
            textDatabase.testTexts[i].data.Korean = textCSV[i]["Korean"].ToString();
            textDatabase.testTexts[i].data.English = textCSV[i]["English"].ToString();
        }
        Debug.Log($"001. {textCSV[0]["English"].ToString()}");
    }
}