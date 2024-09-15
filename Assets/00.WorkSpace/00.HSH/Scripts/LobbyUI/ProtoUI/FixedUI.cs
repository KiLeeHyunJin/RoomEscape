using UnityEngine;
using static UserGameData;

public class FixedUI : MonoBehaviour
{
    [SerializeField] PopUpUI shopUIPrefab;
    [SerializeField] PopUpUI hintShopUIPrefab;
    [SerializeField] PopUpUI albumUIPrefab;
    [SerializeField] PopUpUI optionUIPrefab;
    private void Start()
    {
        Manager.Text.TextChange();
    }
    public void PopUpShopUI()
    {
        Manager.UI.ShowPopUpUI(shopUIPrefab);
    }
    public void PopUpHintUI()
    {
        Manager.UI.ShowPopUpUI(hintShopUIPrefab);
    }
    public void PopUpAlbumUI()
    {
        Manager.UI.ShowPopUpUI(albumUIPrefab);
    }
    public void PopUPOptionUI()
    {
        Manager.UI.ShowPopUpUI(optionUIPrefab);
    }
    public void AllClear()
    {
        for ( int i = 0; i < 5; i++)
        {
            Manager.Data.SetChapterData(i+1, (true, null));
        }
    }
}