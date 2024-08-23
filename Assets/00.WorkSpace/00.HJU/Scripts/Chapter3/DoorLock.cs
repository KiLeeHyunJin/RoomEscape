using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLock : ClickObject
{
    [SerializeField] GameObject lockPuzzle;
    [SerializeField] GameObject doorPuzzle;
    [SerializeField] bool initialized = true;
    [SerializeField] bool check = true;
    [SerializeField] PopUpUI puzzle1;
    [SerializeField] PopUpUI puzzle2;


    public override void ClickEffect()
    {
        switch (state)
        {
            case 0:
                Interact();
                break;

            case 1:
                PopUp();
                break;

            case 2:
                Manager.Game.GameClearPopup();
                Manager.Sound.PlayButtonSound(28);
                break;
        }
    }

    private void Update()
    {
        if (initialized)
        {
            initialized = false;
            doorPuzzle.SetActive(false);
        }
        if (state == 1 && check)
        {
            check = false;
            doorPuzzle.SetActive(true);
            lockPuzzle.SetActive(false);
        }
    }

    public override void InteractEffect()
    {
        state = 1;
        Manager.Chapter.HintData.SetClearQuestion(8);
    }

    public void Puzzle1()
    {
        Manager.UI.ShowPopUpUI(puzzle1);
    }

    public void Puzzle2()
    {
        Manager.UI.ShowPopUpUI(puzzle2);
    }
}
