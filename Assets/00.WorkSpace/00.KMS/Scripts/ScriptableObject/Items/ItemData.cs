/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using LitJson;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ItemData : MonoBehaviour
{
    public List<ScriptableItem> items = new List<ScriptableItem>();

    private void Start()
    {
        BackendChartData.LoadAllChartID();
        StartCoroutine(LoadItemsFromBackend());
    }

    private IEnumerator LoadItemsFromBackend()
    {
        // Backend 서버에서 차트 데이터 불러오기
        var bro = Backend.Chart.GetChartContents(Manager.Data.UserGameData.localChartID);

        // 요청이 완료될 때까지 대기
        yield return bro;

        // 요청이 성공했는지 확인
        if (bro.IsSuccess())
        {
            // JSON 데이터 파싱
            LitJson.JsonData chartJson = JsonMapper.ToObject(Backend.Chart.GetLocalChartData(Manager.Data.UserGameData.localChartID));
            var rows = chartJson["rows"];

            for (int i = 0; i < rows.Count; ++i)
            {
                int tableID = int.Parse(rows[i]["TableID"]["S"].ToString());

                ScriptableItem existingItem = null;

                // itemNum에 해당하는 데이터인지 확인
                if (tableID >= 10000 && tableID < 10050)
                {
                    existingItem = FindScriptableItem(tableID, tableID);
                    if (existingItem != null)
                    {
                        existingItem.itemID = rows[i]["ko"]["S"].ToString(); // 'ko' 필드가 item ID를 포함한다고 가정
                    }
                }
                // descriptionNum에 해당하는 데이터인지 확인
                else if (tableID >= 10051 && tableID < 20000)
                {
                    existingItem = FindScriptableItem(tableID, tableID);
                    if (existingItem != null)
                    {
                        existingItem.description = rows[i]["ko"]["S"].ToString(); // 'ko' 필드가 description을 포함한다고 가정
                    }
                }
                else
                {
                    Message.LogWarning($"TableID {tableID}는 itemNum이나 descriptionNum에 해당하지 않습니다.");
                    continue;
                }

                if (existingItem == null)
                {
                    Message.LogWarning($"TableID {tableID}에 해당하는 ScriptableItem을 찾을 수 없습니다. 업데이트를 건너뜁니다.");
                    continue;
                }

                // 변경 사항 저장
#if UNITY_EDITOR
                EditorUtility.SetDirty(existingItem);
                AssetDatabase.SaveAssets();
#endif
            }
        }
        else
        {
            Message.LogError($"Backend에서 차트 데이터를 불러오는 데 실패했습니다: {bro.GetErrorCode()} - {bro.GetMessage()}");
        }
    }

    private ScriptableItem FindScriptableItem(int itemNum, int descriptionNum)
    {
        string[] guids = AssetDatabase.FindAssets("t:ScriptableItem", new[] { "Assets" });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ScriptableItem item = AssetDatabase.LoadAssetAtPath<ScriptableItem>(path);
            if (item != null && item.itemNum == itemNum || item.descriptionNum == descriptionNum)
            {
                return item;
            }
        }
        return null;

    }
}*/