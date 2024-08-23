using UnityEngine;

public class Fence : ClickObject
{
    [SerializeField] GameObject box;
    [SerializeField] GameObject pond;
    [SerializeField] GameObject fencelock;
    private void Start()
    {
        changeImageValue = 1;
    }
    public override void ClickEffect()
    {
        switch (state)
        {
            case 0:
                PopUp();
                break;
            case 1:
                break;
        }
    }
    private void Update()
    {
        if (state == 1)
        {
            int boxIndex = box.transform.GetSiblingIndex();
            int pondIndex = pond.transform.GetSiblingIndex();

            box.transform.SetSiblingIndex(boxIndex + 1);
            pond.transform.SetSiblingIndex(pondIndex + 1);

            ButtonImage.sprite = changeSprite;
            if ( fencelock != null )
            {
                fencelock.SetActive(false);
            }
        }
    }
}
