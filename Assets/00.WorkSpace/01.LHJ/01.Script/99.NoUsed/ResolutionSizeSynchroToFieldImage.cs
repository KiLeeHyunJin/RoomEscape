using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ResolutionSizeSynchroToFieldImage : MonoBehaviour
{
    float ratio = -1;
#if UNITY_EDITOR
    readonly bool oneShot = false;
#else
    readonly bool oneShot = true;
#endif
    private void Start()
    {                        
        //기기의 높이에 따라 UI 높이 변경
        StartCoroutine(SizeCheck());
    }
    IEnumerator SizeCheck()
    {
        yield return new WaitForEndOfFrame();
        //백그라운드 이미지 레이아웃 찾는다.
        BackGroundSizeSetting layout = GetComponentInChildren<BackGroundSizeSetting>();
        if (layout != null)
        {
            //해당 레이아웃 컴포넌트에서 백그라운드 사이즈 셋팅 컴포넌트를 가져온다.
            float adjustmentValue; //비율을 담아 놓을 변수를 생성한다.
            ReCheck:
            do
            {
                //설정값과 기기값의 비율을 측정한다.
                yield return new WaitForFixedUpdate();

                float heightValue = (layout.transform as RectTransform).rect.height;
                if (heightValue <= 0)
                {
                    //잘못된 상황
                    goto ReCheck;
                }
                else
                {
                    adjustmentValue = Screen.height / heightValue;
                    if (ratio != adjustmentValue)
                    {
                        ratio = adjustmentValue;
                        //비율을 변경한다.
                        //layout.FlowSynch(ratio);
                    }
                }
            }
            while (oneShot == false);
        }
    }

}
