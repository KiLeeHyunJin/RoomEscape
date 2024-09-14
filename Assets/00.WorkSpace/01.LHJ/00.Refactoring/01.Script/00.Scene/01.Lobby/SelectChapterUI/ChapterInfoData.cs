using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object/ChapterInfo", order = int.MaxValue)]
public class ChapterInfoData : ScriptableObject
{
    [SerializeField] int titleId;
    public int TitleId { get { return titleId; } }

    [SerializeField] int descriptionId;
    public int DescriptionId { get { return descriptionId; } }

    [SerializeField] string titleInfo;
    public string TitleInfo { get { return titleInfo; } }

    [SerializeField] string depscriptionInfo;
    public string DepscriptionInfo { get { return depscriptionInfo; } }

    [SerializeField] Sprite icon;
    public Sprite Icon { get { return icon; } }
}
