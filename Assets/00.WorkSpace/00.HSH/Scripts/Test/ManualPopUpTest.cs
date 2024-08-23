using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualPopUpTest : MonoBehaviour
{
    [SerializeField] PopUpUI minigame;

    public void Click()
    {
        Manager.UI.ShowPopUpUI(minigame);
    }
}
