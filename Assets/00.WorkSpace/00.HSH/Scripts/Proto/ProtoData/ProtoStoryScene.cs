using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[CreateAssetMenu(fileName = "NewProtoStoryScene", menuName = "Data/New Proto Story Scene")]
[System.Serializable]
public class ProtoStoryScene : ScriptableObject
{
    public ProtoSS Data = new ProtoSS();
}
public class ProtoSS
{
    public int Id;
    public string KoreanSpeaker;
    public string Korean;
    public string EngliscSpeaker;
    public string English;
    public ProtoSS()
    {
        Id = -1;
        KoreanSpeaker = "";
        Korean = "";
        EngliscSpeaker = "";
        English = "";
    }
}
[System.Serializable]
public class SceneData
{
    public ProtoSS[] Datas;
    public SceneData(ProtoStorySceneDatabase ProtoSSDatabase)
    {
        Array.Resize(ref Datas, ProtoSSDatabase.protoScenes.Length);

        for (int i = 0; i < ProtoSSDatabase.protoScenes.Length; i++)
        {
            Datas[i] = ProtoSSDatabase.protoScenes[i].Data;
        }
    }
}