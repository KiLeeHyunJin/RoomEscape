using UnityEngine;
[CreateAssetMenu(fileName = "New Proto Database", menuName = "Data/New ProtoDatabase")]
public class ProtoStorySceneDatabase : ScriptableObject, ISerializationCallbackReceiver
{
    public ProtoStoryScene[] protoScenes;

    [ContextMenu("Update ID's")]
    public void UpdateID() // 데이터베이스에 들어있는 아이템에 ID 부여
    {
        for (int i = 0; i < protoScenes.Length; i++)
        {
            protoScenes[i].Data.Id = i;
        }
    }
    public void OnAfterDeserialize()
    {
        //UpdateID();
    }

    public void OnBeforeSerialize()
    {

    }
}
