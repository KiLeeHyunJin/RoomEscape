using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Hint", menuName = "Hint System/Hints/hint")]
public class HintObject : ScriptableObject // 힌트 하나
{
    [SerializeField] Sprite sprite;
    [SerializeField] string infomation;

    public string Information { get { return infomation; } }

    public Sprite Sprite { get { return sprite; } }
}
[System.Serializable]

public class Hint//힌트 하나 세트
{
    public int hintId { get; set; }// 힌트 번호
    public bool checkOpened; // 힌트의 오픈 여부
    public string infomation; // 힌트 정보

}