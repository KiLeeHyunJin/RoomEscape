using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Extension
{
    public static void DefaultValue<T>(this T[] array) where T : struct
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = default;
        }
    }

    public static void DefaultCreate<T>(this T[] array) where T : class, new()
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == null)
            {
                array[i] = new();
            }
        }
    }

    /// <summary>
    /// 실행중인 코루틴을 중지하고 다시 실행합니다.
    /// </summary>
    public static void ReStartCoroutine<T>(this T gameObject, IEnumerator routine, ref Coroutine co) where T : MonoBehaviour
    {
        if (co != null)
            gameObject.StopCoroutine(co);
        co = gameObject.StartCoroutine(routine);
    }


    /// <summary>
    /// 해당 레이어의 비트가 올라와있는지 확인해서 반환합니다.
    /// </summary>
    public static bool ContainCheck(this LayerMask layerMask, int layer)
    {
        return ((1 << layer) & layerMask) != 0;
    }

    /// <summary>
    /// 해당 레이어 플래그를 올려줍니다.
    /// </summary>
    public static void Contain(this ref LayerMask layerMask, int layer)
    {
        layerMask |= 1 << layer;
    }

    /// <summary>
    /// 게임오브젝트 자식중 name의 이름을 갖는 객체를 반환합니다.
    /// </summary>
    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform != null)
            return transform.gameObject;
        return null;
    }

    /// <summary>
    /// 게임오브젝트의 자식 중 T타입 요소의 이름이 name인 객체를 찾아 반환합니다.
    /// </summary>
    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            Transform transform = go.transform.Find(name);
            if (transform != null)
                return transform.GetComponent<T>();
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }
        return null;
    }
    /// <summary>
    /// 해당 컴포넌트가 존재한다면 찾아서 반환하고 없을경우 추가합니다.
    /// </summary>
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    static System.Random _rand = new System.Random();

    /// <summary>
    /// 버블소트를 이용해 모든 요소를 섞습니다.
    /// </summary>
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = _rand.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    /// <summary>
    /// 리스트의 크기 범위 내 랜덤한 인덱스를 반환합니다.
    /// </summary>
    public static T GetRandom<T>(this IList<T> list)
    {
        int index = _rand.Next(list.Count);
        return list[index];
    }

    public static void ResetVertical(this ScrollRect scrollRect)
    {
        scrollRect.verticalNormalizedPosition = 1;
    }

    public static void ResetHorizontal(this ScrollRect scrollRect)
    {
        scrollRect.horizontalNormalizedPosition = 1;
    }
}