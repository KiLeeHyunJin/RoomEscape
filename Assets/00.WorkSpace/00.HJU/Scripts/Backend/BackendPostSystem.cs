using BackEnd;
using System.Collections.Generic;
using UnityEngine;
using static UserGameData;

public class BackendPostSystem : MonoBehaviour
{
    private List<PostData> postList = new List<PostData>();

    [ContextMenu("우편 읽기")]

    public void PostRead()
    {
        PostListGet(PostType.Admin);
    }

    [ContextMenu("우편 하나 받기")]
    public void PostGet()
    {
        PostReceive(PostType.Admin, 0);
    }

    /// <summary>
    /// 우편 불러오기
    /// </summary>
    public void PostListGet(PostType postType)
    {
        Backend.UPost.GetPostList(postType, callback =>
        {
            if (!callback.IsSuccess())
            {
                Debug.Log($"우편 받기 실패 : {callback}");
                return;
            }
            else
            {
                Debug.Log($"우편 받기 성공 : {callback}");

                //Json데이터 파싱 성공
                try
                {
                    LitJson.JsonData jsonData = callback.GetFlattenJSON()["postList"];

                    //갯수가 0이면 데이터가 없는 것
                    if (jsonData.Count <= 0)
                    {
                        Debug.Log("우편함이 비어있습니다.");
                        return;
                    }

                    //우편리스트를 불러올 때 postList 초기화
                    postList.Clear();

                    //현재 저장 가능한 모든 우편 정보 불러오기
                    for (int i = 0; i < jsonData.Count; ++i)
                    {
                        PostData post = new PostData();
                        post.title = jsonData[i]["title"].ToString();
                        post.content = jsonData[i]["content"].ToString();
                        post.inDate = jsonData[i]["inDate"].ToString();
                        post.expirationDate = jsonData[i]["expirationDate"].ToString();

                        //우편에 함꼐 발송된 아이템의 차트이름이 "재화차트"일 때
                        foreach (LitJson.JsonData itemJson in jsonData[i]["items"])
                        {
                            if (itemJson["chartName"].ToString() == "재화차트")
                            {
                                string itemName = itemJson["item"]["itemName"].ToString();
                                int itemCount = int.Parse(itemJson["itemCount"].ToString());


                                //우편에 포함된 아이템이 여러개일 때
                                //이미 postReward에 해당  아이템 정보가 있으면 개수 추가
                                if (post.postReward.ContainsKey(itemName))
                                {
                                    post.postReward[itemName] += itemCount;
                                }

                                //postReward에 없는 아이템이면 요소 추가
                                else
                                {
                                    post.postReward.Add(itemName, itemCount);
                                }

                                post.isCanRecive = true;
                            }
                            else
                            {
                                Debug.Log($"아직 지원하지 않는 차트 정보입니다. : {itemJson["chartName"].ToString()}");
                                post.isCanRecive = false;
                            }

                            postList.Add(post);
                        }
                    }

                    //저장 가능한 모든 우편 정보(postList) 출력
                    for (int i = 0; i < postList.Count; ++i)
                    {
                        Debug.Log($"{i}번째 우편\n{postList[i].ToString()}");
                    }
                }

                //Json데이터 파싱 실패
                catch (System.Exception ex)
                {
                    //try-catch 에러 출력
                    Debug.Log(ex);
                }
            }
        });
    }

    /// <summary>
    /// 우편 로컬에 저장
    /// </summary>
    public void SavePostToLocal(LitJson.JsonData item)
    {
        //Json 데이터 파싱 성공
        try
        {
            foreach (LitJson.JsonData itemJson in item)
            {
                //차트파일 이름과 뒤끝 콘솔에 등록한 차트이름 
                string chartFileName = itemJson["item"]["chartFileName"].ToString();
                string chartName = itemJson["chartName"].ToString();

                //차트파일에 등록한 첫번째 행 이름
                int itemId = int.Parse(itemJson["item"]["item"].ToString());
                string itemName = itemJson["item"]["itemName"].ToString();

                //우편 발송할 때 작성하는 아이템 수량
                int itemCount = int.Parse(itemJson["itemCount"].ToString());

                //우편으로 받은 재화를 게임 내 데이터에 적용
                if (chartName.Equals("재화차트"))
                {
                    if (itemName.Equals("hint"))
                    {
                        Manager.Data.UserGameData.SetData(GameDataEnum.Hint, Manager.Data.UserGameData.hint += itemCount);
                    }

                }
                Debug.Log($"{chartName}-{chartFileName}");
                Debug.Log($"[{itemId}]{itemName} : {itemCount}");
            }
        }
        //Json 데이터 파싱 실패
        catch (System.Exception ex)
        {
            //try-catch 에러 출력
            Debug.Log(ex);
        }
    }

    /// <summary>
    /// 우편 하나 수령
    /// </summary>
    public void PostReceive(PostType postType, int index)
    {
        if (postList.Count <= 0)
        {
            Debug.Log("받을 수 있는 우편이 존재하지 않습니다, 혹은 우편 불러오기를 먼저 실행해주세요.");
            return;
        }

        if (index >= postList.Count)
        {
            Debug.Log($"해당 우편은 존재하지 않습니다. : 요청 index{index} / 우편 최대갯수{postList.Count}");
            return;
        }

        Debug.Log($"{postType.ToString()}의 {postList[index].inDate} 우편수령을 요청합니다.");

        Backend.UPost.ReceivePostItem(postType, postList[index].inDate, callback =>
        {
            if (!callback.IsSuccess())
            {
                Debug.Log($"{postType.ToString()}의 {postList[index].inDate} 우편수령 중 에러가 발생했습니다.{callback}");
            }

            Debug.Log($"{postType.ToString()}의 {postList[index].inDate} 우편수령에 성공했습니다.{callback}");
            postList.RemoveAt(index);

            //저장 가능한 아이템이 있을 때
            if (callback.GetFlattenJSON()["postItems"].Count > 0)
            {
                //아이템 저장
                SavePostToLocal(callback.GetFlattenJSON()["postItems"]);
            }
            else
            {
                Debug.Log("수령 가능한 우편 아이템이 존재하지 않습니다.");
            }
        });
    }
}
