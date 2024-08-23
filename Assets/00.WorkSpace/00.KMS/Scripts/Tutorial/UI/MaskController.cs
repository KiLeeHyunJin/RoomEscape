using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaskController : MonoBehaviour
{
    public GameObject maskPanel;
    public RectTransform highlightArea;

    void Start()
    {
        UpdateMask();
    }

    public void SetHighlightArea(RectTransform newHighlightArea)
    {
        highlightArea = newHighlightArea;
        UpdateMask();
    }

    private void UpdateMask()
    {
        RectTransform maskRect = maskPanel.GetComponent<RectTransform>();
        maskPanel.GetComponent<RectTransform>().sizeDelta = highlightArea.sizeDelta;
        maskPanel.GetComponent<RectTransform>().position = highlightArea.position;
    }
}
