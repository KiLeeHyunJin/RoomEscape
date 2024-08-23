using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtoSceneManager : MonoBehaviour
{
    [SerializeField] ProtoStorySceneDatabase ProtoSSDatabase;
    public ProtoBottomBarUI bottomBarUIPrefab;
    void Start()
    {
        //ProtoPlayScene(ProtoSSDatabase.protoScenes[0]);
    }

    //void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
    //    {
    //        if (bottomBarUIPrefab.IsCompleted())
    //        {
    //            if (bottomBarUIPrefab.IsLastSentence())
    //            {

    //                //currentScene = currentScene.nextScene;
    //                //bottomBar.PlayScene(currentScene);
    //            }
    //            else
    //            {
    //                bottomBarUIPrefab.PlayNextSentence();
    //            }
    //        }
    //    }
    //}
    //public void ProtoPlayScene(ProtoStoryScene protoScene)
    //{
    //    Manager.UI.ShowPopUpUI(bottomBarUIPrefab);
    //    bottomBarUIPrefab.currentScene = protoScene;
    //    bottomBarUIPrefab.PlayScene(protoScene);
    //}
}
