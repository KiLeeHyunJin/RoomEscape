using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] Canvas popUpCanvas;
    [SerializeField] Canvas windowCanvas;
    [SerializeField] Canvas inGameCanvas;

    [SerializeField] Image popUpBlocker;
    [SerializeField] Button inGameBlocker;

    Stack<PopUpUI> popUpStack;
    InGameUI curInGameUI;
    //private float prevTimeScale;

    protected override void Awake()
    {
        base.Awake();
        popUpStack = new();
    }
    private void Start()
    {
        EnsureEventSystem();
    }

    public void EnsureEventSystem()
    {
        if (EventSystem.current != null)
            return;
        EventSystem eventSystem = Instantiate(Resources.Load<EventSystem>("UI/EventSystem"));
        eventSystem.transform.SetParent(transform, false);
        EventSystem.current = eventSystem;
    }

    public T ShowPopUpUI<T>(T popUpUI) where T : PopUpUI
    {
        if (popUpStack.Count > 0)
        {
            PopUpUI topUI = popUpStack.Peek();
            topUI.gameObject.SetActive(false);
        }
        else
        {
            popUpBlocker.gameObject.SetActive(true);
            //prevTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }

        T ui = Instantiate(popUpUI, popUpCanvas.transform);
        popUpStack.Push(ui);
        return ui;
    }

    public PopUpUI ShowPopUpUI(string name = null)
    {
        GameObject ui = Manager.Resource.Load<GameObject>($"UI/Popup/{name}");
        if (ui == null)
            return null;

        if (ui.TryGetComponent<PopUpUI>(out PopUpUI baseui))
            return ShowPopUpUI(baseui);
        return null;
    }


    public void ClosePopUpUI()
    {
        if (popUpStack.Count == 0 ||
            popUpStack.TryPop(out PopUpUI ui) == false)
            return;

        Destroy(ui.gameObject);

        if (popUpStack.Count > 0)
        {
            PopUpUI topUI = popUpStack.Peek();
            if (topUI != null)
                topUI.gameObject.SetActive(true);
        }
        else
        {
            popUpBlocker.gameObject.SetActive(false);
            //Time.timeScale = prevTimeScale;
        }
    }
    //현재 팝업 UI가 매개변수랑 같다면 파괴
    //매개변수를 기입하지 않았다면 현재 팝업창을 파괴
    public void ClosePopupUI(PopUpUI popup = null)
    {
        if (popUpStack.Count == 0)
            return;

        if (popup != null)
        {
            if (popUpStack.Peek() == popup)
                ClosePopUpUI();
            else
                Message.Log("Close Popup Failed!");
        }
        else
            ClosePopUpUI();

    }

    public void ClearPopUpUI()
    {
        while (popUpStack.Count > 0)
        {
            ClosePopUpUI();
        }
    }
    //특정 타입의 팝업 UI를 찾아서 반환
    public T FindPopupUI<T>() where T : PopUpUI
    {
        return popUpStack.Where(x => x.GetType() == typeof(T)).FirstOrDefault() as T;
    }

    //현재 팝업 UI를 변환해서 반환
    public T PeekPopupUI<T>() where T : PopUpUI
    {
        return popUpStack.Count == 0 ? null : popUpStack.Peek() as T;
    }

    public T ShowWindowUI<T>(T windowUI) where T : WindowUI
    {
        return Instantiate(windowUI, windowCanvas.transform);
    }

    public void SelectWindowUI(WindowUI windowUI)
    {
        windowUI.transform.SetAsLastSibling();
    }

    public void CloseWindowUI(WindowUI windowUI)
    {
        Destroy(windowUI.gameObject);
    }

    public void ClearWindowUI()
    {
        for (int i = 0; i < windowCanvas.transform.childCount; i++)
        {
            Destroy(windowCanvas.transform.GetChild(i).gameObject);
        }
    }

    public T ShowInGameUI<T>(T inGameUI) where T : InGameUI
    {
        if (curInGameUI != null)
        {
            Destroy(curInGameUI.gameObject);
        }

        T ui = Instantiate(inGameUI, inGameCanvas.transform);
        curInGameUI = ui;
        inGameBlocker.gameObject.SetActive(true);
        return ui;
    }

    public void CloseInGameUI()
    {
        if (curInGameUI == null)
            return;

        inGameBlocker.gameObject.SetActive(false);
        Destroy(curInGameUI.gameObject);
        curInGameUI = null;
    }
}
