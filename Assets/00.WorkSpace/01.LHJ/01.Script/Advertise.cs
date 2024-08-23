using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Advertise : PopUpUI
{
    const string URL = "https://linkz.co.kr/";
    Canvas canvas;
    int sortOrder;
    //bool coolState = false;
    protected override void Awake()
    {
        base.Awake();
        //광고 창이 맨 앞에 출력될 수 있도록 소팅오더 를 재설정
        canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            sortOrder = canvas.sortingOrder;
            canvas.sortingOrder = 10000;
        }
    }

    protected override void Start()
    {
        base.Start();
        //광고창을 누르면 홈페이지로 이동하게 콜백 등록
        Button btn = GetComponentInChildren<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(AdvertiseClose);
        co = StartCoroutine(AutoClose());
    }

    /// <summary>
    /// 광고창을 누르면 창을 닫고 홈페이지로 이동
    /// </summary>
    void AdvertiseClose()
    {
        Debug.Log("Close");
        Close();
        Application.OpenURL(URL);
    }

    private void OnDestroy()
    {
        Out();
    }
    private void OnDisable()
    {
        Out();
    }
     
    Coroutine co;
    /// <summary>
    /// 5초 후 자동 종료
    /// </summary>
    IEnumerator AutoClose()
    {
        yield return new WaitForSeconds(5);
        Close();
    }

    /// <summary>
    /// 종료되었을 시 소팅오더값을 원래값으로 변경
    /// </summary>
    void Out()
    {
        if (canvas != null)
            canvas.sortingOrder = sortOrder;

        if (co != null)
            StopCoroutine(co);
    }
}
