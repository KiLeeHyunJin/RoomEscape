using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HintSlotEntry : BaseUI
{
    enum Images
    {
        IconImg,
        HintSlotPrefab
    }
    enum Texts
    {
        InfomTxt
    }

    public Sprite Icon 
    { 
        set 
        {
            if(iconImage != null)
            iconImage.sprite = value; 
        } 
    }
    public string Information 
    { 
        set 
        { 
            if(informationTxt != null)
            informationTxt.text = value; 
        } 
    }
    public Color IconColor
    {
        set 
        {
            if (iconImage != null)
                iconImage.color = value; 
        }
    }
    public bool RayCastTarget
    {
        set
        {
            castTarget = value;
            if(iconImage != null)
                iconImage.raycastTarget = castTarget;
            if (frameImage != null)
                frameImage.raycastTarget = castTarget;
        }
        get
        {
            return castTarget;
        }
    }
    bool castTarget;
    [SerializeField] TextMeshProUGUI informationTxt;
    [SerializeField] Image iconImage;
    [SerializeField] Image frameImage;

    protected override void Start()
    {
        base.Start();
        this.gameObject.FontInit(Define.Font.MLight);
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindImage(typeof(Images));
        BindText(typeof(Texts));

        if(iconImage == null)
            iconImage = GetImage((int)Images.IconImg);
        if (frameImage == null)
            frameImage = GetImage((int)Images.HintSlotPrefab);
        if (informationTxt == null)
            informationTxt = GetText((int)Texts.InfomTxt);

        return true;
    }


}
