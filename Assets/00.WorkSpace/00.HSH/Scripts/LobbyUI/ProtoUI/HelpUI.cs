using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HelpUI : MonoBehaviour
{
    [SerializeField] GameObject _helpUI;
    public void Click()
    {
        _helpUI.SetActive(false);
    }
}
