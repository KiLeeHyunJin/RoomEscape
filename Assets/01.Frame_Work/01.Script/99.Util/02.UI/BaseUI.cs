using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseUI : MonoBehaviour
{
    protected Dictionary<System.Type, UnityEngine.Object[]> _objects = new Dictionary<System.Type, UnityEngine.Object[]>();

    protected bool _init = false;
    protected virtual void Awake()
    {
        //Bind();
    }
    protected virtual void Start()
    {
        Init();
    }
    public virtual bool Init()
    {
        if (_init)
            return false;

        return _init = true;
    }


    protected void Bind<T>(System.Type type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(type);
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        _objects.Add(typeof(T), objects);

        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
                objects[i] = Utils.FindChild(gameObject, names[i], true);
            else
                objects[i] = Utils.FindChild<T>(gameObject, names[i], true);

            if (objects[i] == null)
                Message.Log($"Failed to bind({names[i]})");
        }
    }

    protected void BindObject(Type type)        { Bind<GameObject>(type); }
    protected void BindImage(Type type)         { Bind<Image>(type); }
    protected void BindText(Type type)          { Bind<TextMeshProUGUI>(type); }
    protected void BindInputField(Type type)    { Bind<TMP_InputField>(type); }
    protected void BindButton(Type type)        { Bind<Button>(type); }

    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        if (_objects.TryGetValue(typeof(T), out UnityEngine.Object[] objects) == false)
            return null;
        return objects[idx] as T;
    }

    protected GameObject GetObject(int idx) { return Get<GameObject>(idx); }
    protected TextMeshProUGUI GetText(int idx) { return Get<TextMeshProUGUI>(idx); }
    protected Button GetButton(int idx) { return Get<Button>(idx); }
    protected Image GetImage(int idx) { return Get<Image>(idx); }

    public static void BindEvent(GameObject go, Action action, Define.UIEvent type = Define.UIEvent.Click)
    {
        UIEventHandler evt = Extension.GetOrAddComponent<UIEventHandler>(go);

        switch (type)
        {
            case Define.UIEvent.Click:
                evt.OnClickHandler -= action;
                evt.OnClickHandler += action;
                break;
            case Define.UIEvent.Pressed:
                evt.OnPressedHandler -= action;
                evt.OnPressedHandler += action;
                break;
            case Define.UIEvent.PointerDown:
                evt.OnPointerDownHandler -= action;
                evt.OnPointerDownHandler += action;
                break;
            case Define.UIEvent.PointerUp:
                evt.OnPointerUpHandler -= action;
                evt.OnPointerUpHandler += action;
                break;
        }
    }

    #region
    //private Dictionary<string, GameObject> gameObjectDic;
    //private Dictionary<string, Component> componentDic;

    //private void Bind()
    //{
    //    Transform[] transforms = GetComponentsInChildren<Transform>(true);
    //    gameObjectDic = new Dictionary<string, GameObject>(transforms.Length * 4);
    //    foreach (Transform child in transforms)
    //    {
    //        gameObjectDic.TryAdd($"{child.gameObject.name}", child.gameObject);
    //    }

    //    Component[] components = GetComponentsInChildren<Component>(true);
    //    componentDic = new Dictionary<string, Component>(components.Length * 4);
    //    foreach (Component child in components)
    //    {
    //        componentDic.TryAdd($"{child.gameObject.name}_{components.GetType().Name}", child);
    //    }
    //}

    //public GameObject GetUI(string name)
    //{
    //    gameObjectDic.TryGetValue(name, out GameObject gameObject);
    //    return gameObject;
    //}

    //public T GetUI<T>(string name) where T : Component
    //{
    //    componentDic.TryGetValue($"{name}_{typeof(T).Name}", out Component component);
    //    if (component != null)
    //        return component as T;

    //    gameObjectDic.TryGetValue(name, out GameObject gameObject);
    //    if (gameObject == null)
    //        return null;

    //    component = gameObject.GetComponent<T>();
    //    if (component == null)
    //        return null;

    //    componentDic.TryAdd($"{name}_{typeof(T).Name}", component);
    //    return component as T;
    //}
    #endregion
}
