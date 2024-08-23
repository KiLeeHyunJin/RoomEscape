using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostData 
{
    public string title;                  //우편제목
    public string content;                //우편내용
    public string inDate;                 //inData
    public string expirationDate;         //만료일

    public bool isCanRecive = false;      //수령가능 여부

    //아이템이름, 아이템개수
    public Dictionary<string, int> postReward = new Dictionary<string, int>();

    public override string ToString()
    {
        string result = string.Empty;
        result += $"title : {title}\n";
        result += $"content : {content}\n";
        result += $"inDate : {inDate}\n";
        result += $"expirationDate : {expirationDate}\n";

        if (isCanRecive)
        {
            result += "우편아이템\n";

            foreach (string itemkey in postReward.Keys)
            {
                result += $"| {itemkey} : {postReward[itemkey]}개\n";
            }
        }
        else
        {
            result += "지원하지 않는 아이템입니다."; 
        }
        return result ;
    }

}
