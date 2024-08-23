using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginBGMButton : MonoBehaviour
{
    public Image imageComponent;
    public Sprite image1;
    public Sprite image2;
    private bool isImage1Active = true; 

    public void SwitchImage()
    {
        isImage1Active = !isImage1Active;
        imageComponent.sprite = isImage1Active ? image1 : image2;
        Manager.Sound.BgmSource.mute = !Manager.Sound.BgmSource.mute;
    }

    public void BgmSourceMute()
    {
        Manager.Sound.BgmSource.mute = true;
    }
}
