/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UseCancelController : MonoBehaviour
{
    public Image useItemImage;
    public Button useItemCancelButton;

    private void Awake()
    {
        useItemCancelButton.onClick.AddListener(Close);
        Close();
    }

    public void OpenImage(Sprite itemSprite)
    {
        useItemImage.sprite = itemSprite;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        useItemImage.sprite = null;
        //Manager.Inventory.OnCancelUseItem();
        gameObject.SetActive(false);
    }
}
*/