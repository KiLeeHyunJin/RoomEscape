using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
[CreateAssetMenu(fileName = "New Question", menuName = "Hint System/Questions/Question")]
public class QuestionObject : ScriptableObject, ISerializationCallbackReceiver
{
    // 이 오브젝트 하나 = 질문 하나
    // 인벤토리 식으로 말하면 이미 아이템이 들어있는 인벤
    public int questionId { get; set; } // 문제 번호 혹은 우선순위
    public bool cleared;
    public HintObject[] hints; // 질문의 힌트들
    [ContextMenu("UpdateHintID")]
    public void UpdateHintID()
    {
        //for (int i = 0; i < hints.Length; i++)
        //{
        //    if (hints[i].data.hintId != i)
        //    {
        //        hints[i].data.hintId = i;
        //    }
        //}
    }
    public void OnBeforeSerialize()
    {
        //UpdateHintID();
    }
    public void OnAfterDeserialize()
    {
        
    }
}