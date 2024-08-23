using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

public class AddScriptToButtons : EditorWindow
{
    [MenuItem("Tools/Add Script To Buttons")]
    static void AddInspectPrefabsOutsideResources()
    {  // 특정 경로에 있는 모든 프리팹 경로를 검색
        string[] allPrefabs = AssetDatabase.FindAssets("t:Prefab", new[] { $"Assets/{Define.workDir}" });

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
        foreach (string prefabPath in prefabsOutsideResources)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null)
            {
                Debug.LogError("No prefab selected!");
                continue;
            }
            ModifyPrefab(prefab);
        }

        string[] allScenes = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });
        foreach (string sceneGUID in allScenes)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            if (sceneAsset != null)
            {
                EditorSceneManager.OpenScene(scenePath);
                AddScriptToSceneButtons();
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            }
        }
    }


    static void AddScriptToSceneButtons()
    {
        Button[] buttons = Resources.FindObjectsOfTypeAll<Button>(); // FindObjectsOfTypeAll 사용
        foreach (Button button in buttons)
        {
            // 오브젝트가 활성화 상태인지 체크하지 않고 스크립트를 추가
            if (!button.gameObject.scene.isLoaded) continue; // 씬에 로드된 오브젝트만 처리

            ClickSound clickSound = GetOrAddComponent<ClickSound>(button.gameObject);
            button.onClick.AddListener(clickSound.PlayClickSound);
        }
    }

    static void ModifyPrefab(GameObject prefab)
    {
        try
        {
            // 프리펩을 수정할 수 있도록 임시 인스턴스를 생성합니다.
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

            CheckAndModifyChildren<Button, ClickSound>(instance.transform);
            // 프리펩을 다시 저장합니다.
            PrefabUtility.SaveAsPrefabAsset(instance, AssetDatabase.GetAssetPath(prefab));
            // 임시 인스턴스를 삭제합니다.
            DestroyImmediate(instance);
        }
        catch (System.Exception ex)
        {
            Debug.Log($"{prefab.name}의 Object 저장 과정에 문제가 발생하였습니다.{ex.Message}");
        }

    }

    static void CheckAndModifyChildren<CheckComponent, AddComponent>(Transform parent) where CheckComponent : Component where AddComponent : Component
    {
        if (parent.gameObject.GetComponent<CheckComponent>() != null)
        {
            GetOrAddComponent<AddComponent>(parent.gameObject);
        }
        foreach (Transform child in parent)
        {
            if (child.gameObject.GetComponent<CheckComponent>() != null)
            {
                GetOrAddComponent<AddComponent>(child.gameObject);
            }
            CheckAndModifyChildren<CheckComponent, AddComponent>(child);
        }
    }

    static T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
        {
            component = ObjectFactory.AddComponent<T>(go);
        }
        return component;
    }
}
