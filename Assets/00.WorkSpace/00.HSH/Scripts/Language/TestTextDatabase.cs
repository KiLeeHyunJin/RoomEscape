using UnityEngine;

[CreateAssetMenu(fileName = " New text DataBase", menuName = "Text System/Texts/Text Database")]
public class TestTextDatabase : ScriptableObject, ISerializationCallbackReceiver
{
    public TestText[] testTexts;

    [ContextMenu("Update ID's")]
    public void UpdateID() // 데이터베이스에 들어있는 스크립트에 ID 부여
    {
        for (int i = 0; i < testTexts.Length; i++)
        {
            if (testTexts[i].data.Id != i)
            {
                testTexts[i].data.Id = i;
            }
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
