using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

public class AddScriptToButtons : EditorWindow
{



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
