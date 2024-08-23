using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CleanMissingScript : EditorWindow
{


    [MenuItem("MyTool/Warning/AllImageSpriteRemove")]
    public static void AllRemoveImg()
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvase in canvases)
        {
            FindImagesInChildren(canvase.transform);
        }
    }

    static void FindImagesInChildren(Transform parent)
    {
        if (parent.TryGetComponent<ResourceRef>(out ResourceRef res))
        {
            if(parent.TryGetComponent<Image>(out Image img))
            {
                img.sprite = null;
            }
        }

        foreach (Transform child in parent)
        {
            if(child.TryGetComponent<Image>(out Image image))
            {
                if (parent.TryGetComponent<Image>(out Image img))
                {
                    img.sprite = null;
                }
            }
            FindImagesInChildren(child);
        }
    }


    [MenuItem("MyTool/Warning/AllTextFontAssetRemove")]
    public static void AllRemoveRef()
    {
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvase in canvases)
        {

        }
    }


    [MenuItem("MyTool/Warning/RemoveMissingScript")]

    public static void Cleaning()
    {
        bool state = UnityEditor.EditorUtility.DisplayDialog(
        "Clean Missing Component",
        $"미싱 컴포넌트를 정리합니다. \n" +
        $"만일 실행을 하면 복구가 불가합니다.",
        "실행", "취소");
        if (state == false)
        {
            Debug.Log("취소");
            return;
        }
        Debug.Log("실행");
        int removeCount = 0;
        GameObject prefab = Selection.activeGameObject;
        if (prefab != null)
            removeCount = CleanMissingScriptsRecursive(prefab.transform);

        PrefabUtility.SavePrefabAsset(prefab);

        UnityEditor.EditorUtility.DisplayDialog(
        "Clean Missing Component",
        $"정리가 완료되었습니다. \n" +
        $"총 삭제된 MissingComponent Count : {removeCount}",
        "확인");
    }

    static int CleanMissingScriptsRecursive(Transform parentTransform)
    {
        int removedCount = 0;

        // 자식 오브젝트 검사
        for (int i = parentTransform.childCount - 1; i >= 0; i--)
        {
            Transform child = parentTransform.GetChild(i);
            removedCount += CleanMissingScriptsRecursive(child);
        }

        // 현재 오브젝트의 컴포넌트 검사
        Component[] components = parentTransform.GetComponents<Component>();
        for (int j = components.Length - 1; j >= 0; j--)
        {
            if (components[j] == null)
            {
                // 미싱된 스크립트 컴포넌트 제거
                Undo.RegisterCompleteObjectUndo(parentTransform.gameObject, "Remove Missing Scripts");
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(parentTransform.gameObject);
                removedCount++;
            }
        }
        return removedCount;
    }

    [MenuItem("MyTool/MissingComponentCheck")]
    static void CheckMissinComponent()
    {
        GameObject prefab = Selection.activeGameObject;
        if (prefab != null)
            CheckMissingComponent(prefab.transform);
    }
    static void CheckMissingComponent(Transform parentTransform, bool remove = false)
    {
        // 자식 오브젝트 검사
        for (int i = parentTransform.childCount - 1; i >= 0; i--)
        {
            Transform child = parentTransform.GetChild(i);

            // 미싱된 컴포넌트 스크립트 찾기
            Component[] components = child.GetComponents<Component>();
            for (int j = components.Length - 1; j >= 0; j--)
            {
                if (components[j] == null)
                {

                    Debug.LogWarning("Removing missing component from " + child.name);
                }
            }
            // 재귀적으로 자식 오브젝트 검사
            CheckMissingComponent(child);
        }
    }
}
