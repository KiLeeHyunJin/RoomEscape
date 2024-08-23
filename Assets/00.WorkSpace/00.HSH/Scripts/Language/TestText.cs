using System;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New text", menuName = "Text System/Texts/Text")]
public class TestText : ScriptableObject
{
    public testT data = new testT();
}
[System.Serializable]

public class testT
{
    public int Id;
    public string Korean;
    public string English;

    public testT()
    {
        Id = -1;
        Korean = "";
        English = "";
    }
    public testT(TestText testText)
    {
        Id = testText.data.Id;
        Korean = testText.data.Korean;
        English = testText.data.English;
    }
}

[System.Serializable]
public class textData
{
    public testT[] testTs;
    public textData(TestTextDatabase testTextDatabase)
    {
        Array.Resize(ref testTs, testTextDatabase.testTexts.Length);

        for (int i = 0; i < testTextDatabase.testTexts.Length; i++)
        {
            testTs[i] = testTextDatabase.testTexts[i].data;
        }
    }
}