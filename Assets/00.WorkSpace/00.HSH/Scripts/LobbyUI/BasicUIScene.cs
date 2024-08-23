using UnityEngine;

public class BasicUIScene : MonoBehaviour
{
    [SerializeField] bool ChapterSearch;
    [SerializeField] Animator SubUIanimator;
    public void Shop()
    {
        Debug.Log("go to Shop");
    }
    public void Album()
    {
        Debug.Log("Album");
    }
    public void Check()
    {
        Debug.Log("chulseolCheck");
    }
    public void Achievement()
    {
        Debug.Log("AchieveScene");
    }
    public void Mail()
    {
        Debug.Log("MailUI");
    }
    public void Board()
    {
        Debug.Log("BoardUI");
    }
    public void ChapterChoice()
    {
        if(ChapterSearch == true)
        {
            ChapterSearch = false;
            SubUIanimator.SetTrigger("Show");
        }
        else
        {
            ChapterSearch = true;
            SubUIanimator.SetTrigger("Hide");
        }
    }
    // 스마트폰 드래그 앤 드롭 연구 필요
}
