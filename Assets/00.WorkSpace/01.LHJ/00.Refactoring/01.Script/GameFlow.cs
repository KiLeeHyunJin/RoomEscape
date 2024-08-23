using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlow : MonoBehaviour
{
    [SerializeField] BaseUI LoginScene;
    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
    void Start()
    {
        RemoveManager();
        Manager.UI.ShowScene("LoginRect");
    }


    void RemoveManager()
    {
        //SceneManager.ReleaseInstance();
    }
}
