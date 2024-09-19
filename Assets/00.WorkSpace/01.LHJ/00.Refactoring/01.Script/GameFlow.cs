using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlow : MonoBehaviour
{
    public enum GameState
    {
        Login,
        Lobby,
        InGame
    }

    [SerializeField] GameObject LoginScene;
    [SerializeField] GameObject LobbyScene;
    [field: SerializeField] public GameState CurrentState { get; private set; }

    private void Start()
    {
        if (Manager.Backend.IsLogin)
            ChangeGameScene(GameState.Lobby);
    }

    public void ChangeGameScene(GameState changeScene)
    {
        LoginScene.SetActive(GameState.Login == changeScene);
        LobbyScene.SetActive(GameState.Lobby == changeScene);
        CurrentState = changeScene;
    }

}
