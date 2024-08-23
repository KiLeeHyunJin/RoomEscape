using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlashChecker : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    Image checkBox;
    Transform checkBoxParent;
    public RectTransform start;
    public RectTransform end;

    public bool currentState { get; private set; }
    Coroutine checkCo;

    void Awake()
    {
        //��ġ ������ �����Ѵ�.
        Init();
    }
    void Init()
    {
        //���� �ʱ�ȭ�Ѵ�.
        checkBox = null;
        //üũ ������ �θ� ���� �������� �����Ѵ�.
        checkBoxParent = start.transform;
        if (start != null)
            CreateCheckBox();
    }

    void CreateCheckBox()
    {
        //üũ �ڽ��� ���ٸ�
        if(checkBox == null)
        {
            //üũ�ڽ� ����
            GameObject obj = new("checkBox");
            checkBox = obj.AddComponent<Image>();
            //���� ���� ����
            obj.transform.SetParent(checkBoxParent);
        }

        //���� �� ������ ����
        checkBox.rectTransform.sizeDelta =
            new Vector2(
                start.sizeDelta.x,
                Vector2.Distance(start.position, end.position) + (start.sizeDelta.y * 2)
            );
        checkBox.rectTransform.position = (start.position + end.position) * 0.5f;

        //ȸ�� ���� ����
        Vector2 direction = (end.position - start.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //���� �������� ���� �ֱ⶧���� -90
        angle -= 90;
        checkBox.rectTransform.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
    //��� ��ư�� �������
    public void OnPointerDown(PointerEventData eventData)
    {
        //��ġ ������ ���������� ��ü ���� ������ Ȯ���Ѵ�.
        if(Vector2.Distance(eventData.position, start.position) < start.sizeDelta.magnitude)
        {
            //üũ ������ ������� Ȯ���Ѵ�.
            checkCo = StartCoroutine(PressCheckRoutine());
            Debug.Log("����!");
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //���� ��ġ ����
        if(checkCo != null)
        {
            //��ġ�� �� �κ��� ������������ Ȯ���Ѵ�.
            if (Vector2.Distance(eventData.position, end.position) < end.sizeDelta.magnitude)
            {
                //endPos = eventData.position;
                StopCoroutine(checkCo);
#if UNITY_EDITOR
                Debug.Log("����!");
#endif
                if (checkBox != null)
                {
                    checkBox.gameObject.SetActive(false);
                    //Destroy(checkBox.gameObject);
                }
            }
        }

    }
    IEnumerator PressCheckRoutine()
    {
        currentState = true;
        do
        {
            //��ġ ������ �̵� ��� �ȿ� ��ġ���ִ��� Ȯ��
            if (CursorCheck(Input.mousePosition))
            {
#if UNITY_EDITOR
                Debug.Log("����!");
#endif
            }
            else
            {
                currentState = false;
#if UNITY_EDITOR
                Debug.Log("Ż��!");
#endif
            }
            yield return null;
        }
        while (currentState);
    }

    bool CursorCheck(Vector2 pos)
    {
        //��ġ ������ üũ ���� ������ Ȯ���Ѵ�.
        return RectTransformUtility.RectangleContainsScreenPoint(checkBox.rectTransform, pos);
    }
}
