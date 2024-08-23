using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UIManager : Singleton<UIManager>
{
    enum Canavas
    {
        SceneCanvas,
        PopUpCanvas,
        EffectCanvas,
    }


    private class ResolutionData
    {
        public ResolutionData(float _width, float _height)
        {
            screenSize.x = originWidth = _width;
            screenSize.y = originHeight = _height;
        }

        readonly public float originWidth;
        readonly public float originHeight;
        readonly public float originRatio;

        public Vector2 screenSize;
    }
    private class SafeAreaData
    {
        public SafeAreaData(Vector2 min, Vector3 max)
        {
            minAnchor = min;
            maxAnchor = max;
        }
        readonly public Vector2 minAnchor;
        readonly public Vector2 maxAnchor;
    }



    public Vector2 SafeAreaAnchorMin { get { return safeArea.minAnchor; } }
    public Vector2 SafeAreaAnchorMax { get { return safeArea.maxAnchor; } }
    public Vector2 ScreenSize { get { return resolution.screenSize; } }
    public float Width { get { return resolution.screenSize.x; } }
    public float Height { get { return resolution.screenSize.y; } }

    [SerializeField] Canvas popUpCanvas;
    [SerializeField] Canvas sceneCanvas;
    [SerializeField] Canvas effecCanvas;
    [SerializeField] Image popUpBlocker;

    private Stack<PopUpUI> popUpStack;

    private ResolutionData resolution;
    private SafeAreaData safeArea;

    private float prevTimeScale;

    protected override void Awake()
    {
        base.Awake();
        resolution = new(Define.S20.x, Define.S20.y);
        popUpStack = new Stack<PopUpUI>();
    }
    private void Start()
    {
        EnsureEventSystem();
        SceneCanvaseResolutionScaler();
        SearchCanvase(Canavas.EffectCanvas, ref effecCanvas);
        SearchCanvase(Canavas.PopUpCanvas, ref popUpCanvas);
        UpdateRatio();
    }


    public void ShowEffect(Transform effectTransform)
    {
        effectTransform.SetParent(effecCanvas.transform);
    }

    //현재 팝업 UI를 변환해서 반환
    public T PeekSceneUI<T>() where T : PopUpUI
    {
        return popUpStack.Count == 0 ? null : popUpStack.Peek() as T;
    }
    //현재 팝업 UI를 변환해서 반환

    void ChangeScene(GameObject scene)
    {
        ClearScene();
        ClosePopUpUI();
        scene.transform.SetParent(sceneCanvas.transform);

        RectTransform rect = scene.transform as RectTransform;
        rect.localScale = Vector3.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;

        if (scene.GetComponent<ResolutionChangeScaler>() == null)
            scene.AddComponent<ResolutionChangeScaler>();
    }

    public bool SceneState()
    {
        if (sceneCanvas.transform.childCount == 0)
            return false;
        return true;
    }

    void ClearScene()
    {
        for (int i = sceneCanvas.transform.childCount; i > 0; i--)
            Destroy(sceneCanvas.transform.GetChild(i - 1).gameObject);
    }

    public BaseUI ShowScene(string name = null, Transform parent = null)
    {
        GameObject ui = Manager.Resource.Load<GameObject>($"UI/Scene/{name}");
        if (ui == null)
            return null;

        if (ui.TryGetComponent<BaseUI>(out BaseUI baseui))
            ChangeScene(Instantiate(ui));

        return baseui;
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

    //팝업 UI 추가
    public T ShowPopUpUI<T>(T popUpUI) where T : PopUpUI
    {
        if (popUpUI == null)
            return null;

        //팝업 UI가 있다면 현재 팝업 UI를 비활성화
        if (popUpStack.Count > 0)
        {
            PopUpUI topUI = popUpStack.Peek();
            topUI.gameObject.SetActive(false);
        }
        else
        {
            //없다면 플로커 UI활성화 
            popUpBlocker.gameObject.SetActive(true);
            prevTimeScale = Time.timeScale;
            //Time.timeScale = 0f;
        }
        //팝업 UI를 복사할 후 부모 오브젝트 설정
        T ui = Instantiate(popUpUI, popUpCanvas.transform);
        ui.gameObject.FontInit(Define.Font.MLight);

        //목록에 올려놓고 복제된 오브젝트를 반환
        popUpStack.Push(ui);
        return ui;
    }

    //현재 팝업 UI 파괴
    public void ClosePopUpUI()
    {
        PopUpUI ui = popUpStack.Pop();
        Destroy(ui.gameObject);

        if (popUpStack.Count > 0)
        {
            PopUpUI topUI = popUpStack.Peek();
            topUI.gameObject.SetActive(true);
        }
        else
        {
            popUpBlocker.gameObject.SetActive(false);
            Time.timeScale = prevTimeScale;
        }

    }
    [ContextMenu("ClearPopup")]
    //모든 팝업창을 파괴
    public void ClearPopUpUI()
    {
        while (popUpStack.Count > 0)
            ClosePopUpUI();
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
                Message.Log("Close Popup Failed!");
            else
                ClosePopupUI();
        }
        else
        {
            popup = popUpStack.Pop();
            if (popup != null)
                Destroy(popup.gameObject);
        }
    }

    void SceneCanvaseResolutionScaler()
    {
        resolution.screenSize = new(Screen.width, Screen.height);
        Vector2 minAnchor = Screen.safeArea.min / ScreenSize;
        Vector2 maxAnchor = Screen.safeArea.max / ScreenSize;
        safeArea = new(minAnchor, maxAnchor);

        if (SearchCanvase(Canavas.SceneCanvas, ref sceneCanvas) == false)
        {
            Message.Log("SearchCanvase Null");
            return;
        }
    }

    public void UpdateRatio()
    {
        if (sceneCanvas.TryGetComponent<CanvasScaler>(out CanvasScaler scaler))
            scaler.referenceResolution = new Vector2(Screen.width, Screen.height);
    }

    bool SearchCanvase(Canavas searchCanvas, ref Canvas canvas)
    {
        bool state = default;
        if (canvas != null)
            state = true;
        else
        {
            string searchCanvasName = searchCanvas.ToString();
            canvas = null;
            foreach (Transform child in transform)
            {
                state = string.Equals(child.gameObject.name, searchCanvasName);
                if (state)
                {
                    canvas = child.GetComponent<Canvas>();
                    break;
                }
            }
        }

        if (state)
        {
            if (sceneCanvas.TryGetComponent<CanvasScaler>(out CanvasScaler scaler))
            {
                if (scaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
                    scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

                scaler.referenceResolution = Define.S20;
            }
            canvas.sortingOrder = 1 * (int)Mathf.Pow(10, (int)searchCanvas);
        }
        return state;
    }
    public void DelEffectObject()
    {
        GameObject[] effectObjects = GetComponentsInChildren<PooledObject>(true).Select(t => t.gameObject).ToArray();
        for(int i = 0; i < effectObjects.Length; i++)
        {
            Destroy(effectObjects[i]);
        }
    }
    void EnsureEventSystem()
    {
        if (EventSystem.current != null)
            return;

        EventSystem eventSystem = Resources.Load<EventSystem>("UI/EventSystem");
        Instantiate(eventSystem);
    }
}
