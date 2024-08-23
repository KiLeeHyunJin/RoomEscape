using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class C5Drawer : ClickObject
{
    [SerializeField] ClickObject Up;
    [SerializeField] ClickObject Under;

    [SerializeField] Sprite[] sprites;
    [SerializeField] Image myImage;
    private void Start()
    {
        myImage = GetComponent<Image>();
    }
    private void Update()
    {
        if(Up.state ==0 &&  Under.state ==0 )
        {
            myImage.sprite = sprites[0]; 
        }
        else if (Up.state == 0 && Under.state == 1)
        {
            myImage.sprite = sprites[1];
        }
        else if ( Up.state == 1 && Under.state == 0 )
        {
            myImage.sprite = sprites[2];
        }
        else
        {
            myImage.sprite = sprites[3];
        }
    }
}
