using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Require : EditorWindow
{
    [UnityEditor.MenuItem("MyTool/RefBundle/InScence_AddRef")]
    public static void RefResource()
    {
        AddComponents();
    }

    [UnityEditor.MenuItem("MyTool/RefBundle/InScence_RemoveRef")]
    public static void RemoveRef()
    {
        RemoveComponents();
    }



    private static void RemoveComponents()
    {
        ResourceRef[] allComponents = ResourceRef.FindObjectsOfType<ResourceRef>();
        foreach (ResourceRef component in allComponents)
            DestroyImmediate(component);

        UnityEditor.EditorUtility.DisplayDialog("ResourceRef Remove", $"{TwoFormat(allComponents.Length)}개의 ResourceRef를 삭제하였습니다.", "완료");
    }


    private static void AddComponents()
    {
        (int imgCount, int txtCount) = (0, 0);
        AssetBundle.UnloadAllAssetBundles(true);
        string json = File.ReadAllText($"{Define.dir}{Define.bundleTable}");
        BundleTable table = JsonUtility.FromJson<BundleTable>(json);

        foreach (GameObject go in FindObjectsOfType<GameObject>())
        {
            if (go.GetComponent<Image>() != null)
            {
                AddRefComponent(go,go.name, in table);
                imgCount++;
            }
            //else if (go.GetComponent<TextMeshProUGUI>() != null)
            //{
            //    AddRefComponent(go, go.name, in table);
            //    txtCount++;
            //}
        }

        UnityEditor.EditorUtility.DisplayDialog(
            "ResourceRef Refresh",
            $"{TwoFormat(imgCount)}개의 이미지 정보를 최신화하였습니다.\n{TwoFormat(txtCount)}개의 텍스트 정보를 최신화하였습니다.",
            "완료");
        AssetBundle.UnloadAllAssetBundles(true);
    }



    [UnityEditor.MenuItem("MyTool/RefBundle/Prefab_RemoveTextRef")]
    static void RemoveInspectPrefabsOutsideText()
    {
        string[] allPrefabs = AssetDatabase.FindAssets("t:Prefab", new[] { $"Assets/{Define.workDir}" });
        List<string> prefabsInTargetPath = new();
        List<string> prefabsOutsideResources = new();
        foreach (string prefabGUID in allPrefabs)
            prefabsInTargetPath.Add(AssetDatabase.GUIDToAssetPath(prefabGUID));
        foreach (string prefabGUID in allPrefabs)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUID);
            if (!prefabPath.StartsWith("Assets/Resources"))
                prefabsOutsideResources.Add(prefabPath);
        }

        int totalCount = 0;
        foreach (string prefabPath in prefabsOutsideResources)
        {
            int count = 0;
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            // 프리팹이 null이 아닌 경우 검사 시작
            if (prefab != null)
            {
                foreach (ResourceRef com in prefab.GetComponentsInChildren<ResourceRef>())
                {
                    if(com.GetComponent<TextMeshPro>() != null)
                    {
                        DestroyImmediate(com.GetComponent<ResourceRef>(), true);
                        com.gameObject.name = com.gameObject.name.Split('@')[0];
                        count++;
                    }
                }
                PrefabUtility.SavePrefabAsset(prefab);
            }
            totalCount += count;
        }
        UnityEditor.EditorUtility.DisplayDialog(
       "ResourceRef Check And AddComponent",
       $"{TwoFormat(prefabsOutsideResources.Count)}개의 프리펩에 할당된 ResourceRef컴포넌트를 삭제하였습니다.\n" +
       $"총 삭제 횟수 : {totalCount}",
       "완료");
    }



    [UnityEditor.MenuItem("MyTool/RefBundle/Prefab_RemoveRef")]
    static void RemoveInspectPrefabsOutsideResources()
    {
        string[] allPrefabs = AssetDatabase.FindAssets("t:Prefab", new[] { $"Assets/{Define.workDir}" });
        List<string> prefabsInTargetPath = new();
        List<string> prefabsOutsideResources = new();
        foreach (string prefabGUID in allPrefabs)
            prefabsInTargetPath.Add(AssetDatabase.GUIDToAssetPath(prefabGUID));
        foreach (string prefabGUID in allPrefabs)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUID);
            if (!prefabPath.StartsWith("Assets/Resources"))
                prefabsOutsideResources.Add(prefabPath);
        }

        int totalCount = 0;
        foreach (string prefabPath in prefabsOutsideResources)
        {
            int count = 0;
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            // 프리팹이 null이 아닌 경우 검사 시작
            if (prefab != null)
            {
                foreach (ResourceRef com in prefab.GetComponentsInChildren<ResourceRef>())
                {
                    if (com == null)
                        continue;
                    DestroyImmediate(com.GetComponent<ResourceRef>(), true);
                    count++;
                }
                PrefabUtility.SavePrefabAsset(prefab);
            }
            totalCount += count;
        }
        UnityEditor.EditorUtility.DisplayDialog(
       "ResourceRef Check And AddComponent",
       $"{TwoFormat(prefabsOutsideResources.Count)}개의 프리펩에 할당된 ResourceRef컴포넌트를 삭제하였습니다.\n" +
       $"총 삭제 횟수 : {totalCount}",
       "완료");
    }

    [UnityEditor.MenuItem("MyTool/RefBundle/Prefab_AddRef")]
    static void AddInspectPrefabsOutsideResources()
    {  // 특정 경로에 있는 모든 프리팹 경로를 검색
        string[] allPrefabs = AssetDatabase.FindAssets("t:Prefab", new[] { $"Assets/{Define.workDir}" });
        // 특정 경로에 있는 프리팹들을 저장할 리스트
        string json = File.ReadAllText($"{Define.dir}{Define.bundleTable}");
        BundleTable table = JsonUtility.FromJson<BundleTable>(json);

        List<string> prefabsInTargetPath = new();
        List<string> prefabsOutsideResources = new();
        foreach (string prefabGUID in allPrefabs)
            prefabsInTargetPath.Add(AssetDatabase.GUIDToAssetPath(prefabGUID));

        // Resources 폴더 외부에 있는 프리팹들을 저장할 리스트

        foreach (string prefabGUID in allPrefabs)
        {
            // 프리팹 경로 가져오기
            string prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUID);

            // Resources 폴더 외부에 있는지 검사
            if (!prefabPath.StartsWith("Assets/Resources"))
            {
                prefabsOutsideResources.Add(prefabPath);
            }
        }
        int totalCount = 0;
        foreach (string prefabPath in prefabsOutsideResources)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);//Selection.activeGameObject;
            if (prefab == null)
            {
                Debug.LogError("No prefab selected!");
                continue;
            }
            int count = 0;
            ModifyPrefab(prefab, table, ref count);
            totalCount += count;
        }
        UnityEditor.EditorUtility.DisplayDialog(
        "ResourceRef Check And AddComponent",
        $"{TwoFormat(prefabsOutsideResources.Count)}개의 프리펩의 정보를 검사 및 정보를 최신화하였습니다.\n" +
        $"총 검사 횟수 : {totalCount}",
        "완료");
    }

    static void ModifyPrefab(GameObject prefab, in BundleTable table, ref int repeat)
    {
        // 프리펩을 수정할 수 있도록 임시 인스턴스를 생성합니다.
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        try
        {
            CheckAndModifyChildren(instance.transform, instance.name, table, ref repeat);
            // 프리펩을 다시 저장합니다.
            PrefabUtility.SaveAsPrefabAsset(instance, AssetDatabase.GetAssetPath(prefab));
        }
        catch (Exception ex)
        {
            Debug.Log($"{instance.name}의 Object 저장 과정에 문제가 발생하였습니다.{ex.Message}");
        }
        // 임시 인스턴스를 삭제합니다.
        DestroyImmediate(instance);
    }

    static void CheckAndModifyChildren(Transform parent, string parenName, in BundleTable table, ref int repeat)
    {
        if (parent.gameObject.GetComponent<Image>() != null)
        {
            AddRefComponent(parent.gameObject, parenName, in table, true);
            repeat++;
        }
        foreach (Transform child in parent)
        {
            if (child.gameObject.GetComponent<Image>() != null)
            {
                AddRefComponent(child.gameObject, parenName,in table, true);
                repeat++;
            }
            CheckAndModifyChildren(child, parenName, table, ref repeat);
        }
    }


    static void AddRefComponent(GameObject go,string parenName,in BundleTable table, bool prefab = false)
    {
        ResourceRef resoureRef = go.GetComponent<ResourceRef>();
        if (resoureRef == null)
        {
            resoureRef = prefab ?
            ObjectFactory.AddComponent<ResourceRef>(go) : go.AddComponent<ResourceRef>();
        }
        resoureRef.Init( in table, parenName, prefab);
    }

   

    static string TwoFormat(int count)
    {
        return string.Format("{0:D2}", count);
    }

}






