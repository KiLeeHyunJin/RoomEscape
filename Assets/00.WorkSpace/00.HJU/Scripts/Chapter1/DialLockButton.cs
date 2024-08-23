using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialLockButton : MonoBehaviour
{
    [SerializeField] DialLock dialLock;
    public int index = 0;

    public void ChangeNumber()
    {
        TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
        index = (index + 1) % dialLock.numbers.Length;
        text.text = dialLock.numbers[index];
        dialLock.CheckAnswer();
    }
}
