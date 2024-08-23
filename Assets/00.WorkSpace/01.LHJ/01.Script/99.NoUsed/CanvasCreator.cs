
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class CanvasCreator : MonoBehaviour
{
    //생성할 캔버스의 주소
    [SerializeField] AssetReferenceGameObject Canvas;
    //다운로드 받을 레이블
    [SerializeField] string[] canvasName;
    Transform canvas;
    void Start()
    {
        //레이블을 갖고 있는 어드레서블 에셋을 다운로드
        //Manager.DownLoad.DownLoad(Init, canvasName);
    }

}
