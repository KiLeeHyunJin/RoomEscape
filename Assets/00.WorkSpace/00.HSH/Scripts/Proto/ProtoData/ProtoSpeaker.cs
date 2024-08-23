using UnityEngine;

[CreateAssetMenu(fileName = "NewProtoSpeaker", menuName = "Data/New Proto Speaker")]
[System.Serializable]
public class ProtoSpeaker : ScriptableObject
{
    public string speakerName;
    public Color textColor;
}
