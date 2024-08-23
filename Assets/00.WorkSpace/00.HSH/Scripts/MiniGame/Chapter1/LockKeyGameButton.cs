using TMPro;
using UnityEngine;

public class LockKeyGameButton : MonoBehaviour
{
    [SerializeField] LockKeyGame lockKeygame;
    public int index = 0;

    private void Start()
    {
        TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
        text.text = lockKeygame.numbers[index];
    }

    public void ChangeNumber()
    {
        TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
        index = (index + 1) % lockKeygame.numbers.Length;
        text.text = lockKeygame.numbers[index];
        lockKeygame.CheckAnswer();
    }
}
