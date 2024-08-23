using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScene : ScriptableObject
{
    public List<Sentence> sentences;
    public Sprite background;
    public GameScene nextScene;

    [System.Serializable]
    public struct Sentence
    {
        public string text;
        public Speaker speaker;
    }
}