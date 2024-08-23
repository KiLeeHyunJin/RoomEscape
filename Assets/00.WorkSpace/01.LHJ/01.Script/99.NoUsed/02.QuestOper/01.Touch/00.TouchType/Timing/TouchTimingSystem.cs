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
        //��Ʈ ����
        timingButtonRef.InstantiateAsync().Completed += (obj) =>
        {
            //�Ҵ� ������ ���� ����
            timingObj = obj.Result;
            pooled = null;
            //������Ʈ Ǯ�� ���� ������Ʈ ����
            if (timingObj.TryGetComponent<PooledObject>(out PooledObject instanceObj))
                GetInstancObject(instanceObj);
            //���� ��ƾ ����
            StartSpawnRoutine();
        };
    }
    //������Ʈ Ǯ�� �Ҵ�
    void GetInstancObject(PooledObject obj)
    {
        if(obj.TryGetComponent(out TouchTiming touch))
        {
            pooled = touch;
            Manager.Pool.CreatePool(touch, 10, 10);
            //Ÿ�̹� üũ�� ���� ��ųʸ� �ʱ�ȭ
            CheckDictionaryInit();
        }
    }
    //Ÿ�̹� ��ü�� �����ϴ� ��ƾ�� ����
    //���� ��� ����, ���� ���� , ���� �ֱ�
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

    // Ư�� ���� ��� ������ ���ϴ��� Ȯ���ϴ� �Լ�
    public TouchTiming.TouchTimingType CheckValueRange(float value)
    {
        //�̽� üũ
        if(minSize > value || maxSize < value)
            return TouchTiming.TouchTimingType.Miss;
        //Ÿ�̹� üũ
        foreach (var range in timingTypeDic)
        {
            Vector2 sideValueCheck = range.Key;
            if (sideValueCheck.x > value && value >= sideValueCheck.y)
                return range.Value;
        }
        return TouchTiming.TouchTimingType.Miss;
    }
    //�˻��� ������ �Ҵ�
    void CheckDictionaryInit()
    {
        //��ųʸ� �ʱ�ȭ
        timingTypeDic.Clear();
        //���� ������ ����
        float sizeDelta = (timingObj.transform as RectTransform).sizeDelta.x;
        //������ ���� ��� �ʱ�ȭ
        float multiple = 1.5f;
        //�ּ� ���� ũ�� ����
        maxSize = sizeDelta * multiple;
        //�ִ� ���� ũ�� ����
        minSize = sizeDelta;

        //�̽� ���� ����
        timingTypeDic.Add(new Vector2( float.MaxValue, maxSize), TouchTiming.TouchTimingType.Miss);
        //�ݺ������� ������ �� ����
        TouchTiming.TouchTimingType[] setArray = new TouchTiming.TouchTimingType[] {
            TouchTiming.TouchTimingType.Bad,
            TouchTiming.TouchTimingType.Normal, 
            TouchTiming.TouchTimingType.Good, 
            TouchTiming.TouchTimingType.Excellent,
            TouchTiming.TouchTimingType.Perferct};

        //�ݺ����� ���鼭 �ش� Ÿ�Կ� �´� ���� �� ����
        foreach (TouchTiming.TouchTimingType type in setArray)
        {
            float maxValue = sizeDelta * multiple;
            multiple -= 0.1f;
            float minValue = sizeDelta * multiple;
            timingTypeDic.Add(new Vector2(maxValue, minValue), type);
        }
    }


    //Ÿ�̹� ��ư�� ���� ��ƾ�� �����Ѵ�.
    IEnumerator SpawnRoutine(bool speedTouch)
    {
        foreach (float  rateTime in spawnRate)
        {
            //��ư�� �����Ѵ�.
            PooledObject getPool =
                Manager.Pool.GetPool(pooled,
                new Vector3(
                    UnityEngine.Random.Range(minSize, Screen.width - minSize),
                    UnityEngine.Random.Range(minSize, Screen.height - minSize)));

            //�������� ����
            getPool.transform.SetParent(transform);
            getPool.transform.SetAsFirstSibling();

            //Ǯ ������Ʈ �ʱ�ȭ
            if (getPool.TryGetComponent(out TouchTiming instance))
            {
                instance.SetCheckAction(CheckValueRange);
                instance.Init(speedTouch);

                //���� ��尡 �ƴϸ� ��ü�� ��Ƴ��� ���� ����̸� ���� �ð��� ���
                if (speedTouch == false)
                    touchTimings.Add(instance);
                else
                    yield return new WaitForSeconds(rateTime);
            }
        }
    }


    private void OnDestroy()
    {
        //������ ��ü���� �Ҵ� �����Ѵ�.
        Manager.Pool.DestroyPool(pooled);
        Addressables.ReleaseInstance(timingObj);
    }

}
