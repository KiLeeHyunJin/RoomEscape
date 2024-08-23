using System;
using System.Collections.Generic;

[System.Serializable]

public struct ChapterGameData
{
    public List<bool> isCleared;
    public List<bool> isFirst;
    public List<WrongImageData> wrongImageData;

    public void Init()
    {
        int count = (int)DataManager.GameDataName.END;

        isCleared = new(count);
        isFirst = new(count);

        ListInit(isFirst);
        ListInit(isCleared);
        InitWrongImageData(count);
    }
    public void InitWrongImageData(int count)
    {
        wrongImageData ??= new(count);
        
        if(wrongImageData.Count <= count)
        {
            for (int i = wrongImageData.Count; i < count; i++)
            {
                wrongImageData.Add(new());
            }
        }

        for (int i = 0; i < count; i++)
        {
            wrongImageData[i].Init();
        }
    }
    void ListInit(List<bool> list)
    {
        for (int i = 0; i < list.Capacity; i++)
            list.Add(false);
    }
}

[Serializable]
public class WrongImageData
{
    public bool[] clearedState;

    public bool? this [int idx] 
    { 
        get 
        {
            if (idx >= Define.WrongImageStageCount)
                return null;

            if (clearedState == null)
                Init();

            return clearedState[idx]; 
        }
        set 
        {
            if (idx >= Define.WrongImageStageCount)
                return;

            if (clearedState == null)
               Init();

            clearedState[idx] = value.GetValueOrDefault(); 
        } 
    }

    public void Init()
    {
        if (clearedState == null ||
            clearedState.Length < Define.WrongImageStageCount)
        {
            clearedState = new bool[Define.WrongImageStageCount];
        }

        for (int i = 0; i < Define.WrongImageStageCount; i++)
        {
            if (clearedState[i])
                clearedState[i] = false;
        }
    }
}
