using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxPuzzleButton : MonoBehaviour
{
    public Button button;
    public Sprite pressedSprite; // 눌린 상태의 Sprite
    public Sprite normalSprite; // 눌리지 않은 상태의 Sprite

    public Image buttonImage;

    private bool isPressed = false;
    void Start()
    {
        buttonImage = button.GetComponent<Image>();
        button.onClick.AddListener(ToggleButtonState);
        UpdateButtonSprite(); // 초기 상태 Sprite 설정
        Debug.Log($"{gameObject.name} : {isPressed}");
    }

    void ToggleButtonState()
    {
        isPressed = !isPressed;
        Debug.Log($"{gameObject.name} 토글 : {isPressed}");
        UpdateButtonSprite(); // 버튼 상태가 바뀔 때마다 Sprite 업데이트
        BoxPuzzle.Instance.CheckPuzzle(); // 퍼즐 완료 여부 체크
    }

    public void SetPressed(bool pressed)
    {
        isPressed = pressed;
        UpdateButtonSprite(); // 상태 설정 시 Sprite 업데이트
        Debug.Log($"{gameObject.name} 의 {isPressed}");
    }

    public void UpdateButtonSprite()
    {
        buttonImage.sprite = isPressed ? pressedSprite : normalSprite;
    }

    public bool IsPressed()
    {
        return isPressed;
    }
}
