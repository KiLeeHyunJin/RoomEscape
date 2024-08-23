using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LightOutCell : MonoBehaviour, IPointerDownHandler
{
    private LightOutPuzzlePopup puzzle;
    private int x, y;
    private bool isOn;
    private Image image;


    public void Setup(LightOutPuzzlePopup puzzle, int x, int y)
    {
        this.puzzle = puzzle;
        this.x = x;
        this.y = y;
        image = GetComponent<Image>();
        isOn = false;
        UpdateColor();
        transform.localScale = Vector3.one;
    }


    public void Toggle()
    {
        isOn = !isOn;
        UpdateColor();
    }

    private void UpdateColor()
    {
        if (isOn)
        {
            image.color = puzzle.onColor;
        }
        else
        {
            image.color = puzzle.offColor;
        }
    }

    public bool IsOn()
    {
        return isOn;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        puzzle.CellClicked(x, y);
    }
}
