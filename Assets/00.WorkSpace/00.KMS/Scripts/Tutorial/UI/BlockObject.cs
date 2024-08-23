using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockObject : MonoBehaviour
{
    public Transform block; // 블록의 위치
    public GameObject[] inGameObjects; // 가려야할 인게임 오브젝트들

    void Start()
    {
        foreach (var inGameObject in inGameObjects)
        {
            ClickObject clickObject = inGameObject.GetComponent<ClickObject>();
            if (clickObject != null)
            {
                EventTrigger trigger = inGameObject.GetComponent<EventTrigger>();
                if (trigger == null)
                {
                    trigger = inGameObject.AddComponent<EventTrigger>();
                }

                EventTrigger.Entry entry = new EventTrigger.Entry
                {
                    eventID = EventTriggerType.PointerClick
                };
                entry.callback.AddListener((eventData) => { OnClickObject(clickObject.transform); });
                trigger.triggers.Add(entry);
            }
        }
    }

    public void MoveBlockToFront(Transform clickObjectTransform) // 블록을 앞으로 이동
    {
        int idx = block.transform.GetSiblingIndex() - 1;
        if(idx > -1 )
        {
            block.transform.SetSiblingIndex(idx);
        }
    }

    private void OnClickObject(Transform clickObjectTransform)
    {
        MoveBlockToFront(clickObjectTransform);
    }

}