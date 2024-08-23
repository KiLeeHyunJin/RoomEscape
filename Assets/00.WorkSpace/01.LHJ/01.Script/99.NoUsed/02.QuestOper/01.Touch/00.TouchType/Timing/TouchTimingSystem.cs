using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class TouchTimingSystem : MonoBehaviour
{
    [SerializeField] float[] spawnRate;
    [SerializeField] AssetReferenceGameObject timingButtonRef;
    Dictionary<Vector2,TouchTiming.TouchTimingType> timingTypeDic = new Dictionary<Vector2, TouchTiming.TouchTimingType>();
    GameObject timingObj;
    PooledObject pooled;
    List<TouchTiming> touchTimings;
    float minSize;
    float maxSize;
    private void Awake()
    {
        //노트 생성
        timingButtonRef.InstantiateAsync().Completed += (obj) =>
        {
            //할당 해제를 위해 저장
            timingObj = obj.Result;
            pooled = null;
            //오브젝트 풀을 위해 컴포넌트 참조
            if (timingObj.TryGetComponent<PooledObject>(out PooledObject instanceObj))
                GetInstancObject(instanceObj);
            //스폰 루틴 시작
            StartSpawnRoutine();
        };
    }
    //오브젝트 풀링 할당
    void GetInstancObject(PooledObject obj)
    {
        if(obj.TryGetComponent(out TouchTiming touch))
        {
            pooled = touch;
            Manager.Pool.CreatePool(touch, 10, 10);
            //타이밍 체크를 위한 딕셔너리 초기화
            CheckDictionaryInit();
        }
    }
    //타이밍 객체를 생성하는 루틴을 시작
    //리듬 모드 유무, 생성 개수 , 생명 주기
    public void StartSpawnRoutine(bool speedTouch = true,int count = 0, float outTime = 5)
    {
        if (speedTouch == false)
        {
            touchTimings = new List<TouchTiming>(count);
            spawnRate = new float[count];
            StartCoroutine(RemoveTimeRoutine(outTime));
        }
        StartCoroutine(SpawnRoutine(speedTouch));
    }

    IEnumerator RemoveTimeRoutine(float removeTime)
    {
        yield return new WaitForSeconds(removeTime);
        foreach (TouchTiming note in touchTimings)
        {
            note.PopNote();
        }
    }

    // 특정 값이 어느 범위에 속하는지 확인하는 함수
    public TouchTiming.TouchTimingType CheckValueRange(float value)
    {
        //미스 체크
        if(minSize > value || maxSize < value)
            return TouchTiming.TouchTimingType.Miss;
        //타이밍 체크
        foreach (var range in timingTypeDic)
        {
            Vector2 sideValueCheck = range.Key;
            if (sideValueCheck.x > value && value >= sideValueCheck.y)
                return range.Value;
        }
        return TouchTiming.TouchTimingType.Miss;
    }
    //검사할 값들을 할당
    void CheckDictionaryInit()
    {
        //딕셔너리 초기화
        timingTypeDic.Clear();
        //기준 사이즈 저장
        float sizeDelta = (timingObj.transform as RectTransform).sizeDelta.x;
        //점수에 따른 배수 초기화
        float multiple = 1.5f;
        //최소 점수 크기 저장
        maxSize = sizeDelta * multiple;
        //최대 점수 크기 저장
        minSize = sizeDelta;

        //미스 기준 저장
        timingTypeDic.Add(new Vector2( float.MaxValue, maxSize), TouchTiming.TouchTimingType.Miss);
        //반복문으로 설정할 값 저장
        TouchTiming.TouchTimingType[] setArray = new TouchTiming.TouchTimingType[] {
            TouchTiming.TouchTimingType.Bad,
            TouchTiming.TouchTimingType.Normal, 
            TouchTiming.TouchTimingType.Good, 
            TouchTiming.TouchTimingType.Excellent,
            TouchTiming.TouchTimingType.Perferct};

        //반복문을 돌면서 해당 타입에 맞는 기준 값 저장
        foreach (TouchTiming.TouchTimingType type in setArray)
        {
            float maxValue = sizeDelta * multiple;
            multiple -= 0.1f;
            float minValue = sizeDelta * multiple;
            timingTypeDic.Add(new Vector2(maxValue, minValue), type);
        }
    }


    //타이밍 버튼의 스폰 루틴을 실행한다.
    IEnumerator SpawnRoutine(bool speedTouch)
    {
        foreach (float  rateTime in spawnRate)
        {
            //버튼을 스폰한다.
            PooledObject getPool =
                Manager.Pool.GetPool(pooled,
                new Vector3(
                    UnityEngine.Random.Range(minSize, Screen.width - minSize),
                    UnityEngine.Random.Range(minSize, Screen.height - minSize)));

            //객층구조 조정
            getPool.transform.SetParent(transform);
            getPool.transform.SetAsFirstSibling();

            //풀 오브젝트 초기화
            if (getPool.TryGetComponent(out TouchTiming instance))
            {
                instance.SetCheckAction(CheckValueRange);
                instance.Init(speedTouch);

                //리듬 모드가 아니면 객체를 모아놓고 리듬 모드이면 스폰 시간을 대기
                if (speedTouch == false)
                    touchTimings.Add(instance);
                else
                    yield return new WaitForSeconds(rateTime);
            }
        }
    }


    private void OnDestroy()
    {
        //스폰한 객체들을 할당 해제한다.
        Manager.Pool.DestroyPool(pooled);
        Addressables.ReleaseInstance(timingObj);
    }

}
