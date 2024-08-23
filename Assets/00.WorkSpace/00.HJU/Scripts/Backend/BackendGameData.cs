using BackEnd;
using System;
using UnityEngine;
using UnityEngine.Events;
public class BackendGameData
{
    [System.Serializable]
    public class GameDataLoadEvent : UnityEvent { }
    public GameDataLoadEvent onGameDataLoadEvent = new GameDataLoadEvent();

    //외부에서 함수에 접근하도록 get 선언
    private static BackendGameData instance = null;
    public static BackendGameData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new BackendGameData();
            }
            return instance;
        }
    }


    //불러온 게임 정보의 고유 값 저장
    private string gameDataRowInData = string.Empty;
    private string chapterDataRowInData = string.Empty;
    private string chapterHintRowInData_1 = string.Empty;
    private string chapterHintRowInData_2 = string.Empty;
    private string chapterHintRowInData_3 = string.Empty;
    private string chapterHintRowInData_4 = string.Empty;
    private string chapterHintRowInData_5 = string.Empty;
    private string chapterClearRowInData = string.Empty;

    /// <summary>
    /// 계정 생성할 때 기본 게임정보 로컬에 저장, 뒤끝콘솔 테이블에 업데이트
    /// </summary>
    public void GameDataInsert()
    {
        //게임정보 로컬에 저장
        //Manager.Data.SaveData(DataManager.DataName.UserData);
        Manager.Data.UserGameData.Reset();

        //테이블에 추가할 데이터로 가공
        Param param = new Param()
        {
            {"localChartID",    Manager.Data.UserGameData.localChartID},
            {"chapter",         Manager.Data.UserGameData.chapter},
            {"hint",            Manager.Data.UserGameData.hint},
            {"sfx",             Manager.Data.UserGameData.sfx},
            {"bgm",             Manager.Data.UserGameData.bgm},
            {"kor",             Manager.Data.UserGameData.kor},
            {"lobbyInfo",       Manager.Data.UserGameData.lobbyInfo},
            {"currentVer",      Manager.Data.UserGameData.currentVer},
};

        //첫 번째 매개변수는 뒤끝 콘솔의 "게임 정보 관리" 탭에 생성한 테이블 이름
        Backend.GameData.Insert("User_Data", param, callback =>
        {
            //게임 정보 추가에 성공했을 때
            if (callback.IsSuccess())
            {
                //게임 정보의 고유값
                gameDataRowInData = callback.GetInDate();
                Debug.Log($"게임 정보에 데이터 삽입 완료 : {callback}");

                //게임 정보 추가에 실패했을 때
            }
            else
            {
                Debug.Log($"게임 정보에 데이터 삽입 실패 : {callback}");
            }
        });
    }

    /// <summary>
    /// 뒤끝에 챕터데이터, 챕터힌트, 챕터클리어 생성
    /// </summary>
    /// <param name="data"></param>
    public void AllChapterDataInsert(string data = "")
    {
        // Chapter_Data 테이블에 데이터 삽입
        Param chapterParam = new Param()
    {
        {"chapterData", data},
    };

        Backend.GameData.Insert("Chapter_Data", chapterParam, callback =>
        {
            if (callback.IsSuccess())
            {
                chapterDataRowInData = callback.GetInDate();
                Debug.Log($"Chapter_Data에 데이터 삽입 완료 : {callback}");
            }
            else
            {
                Debug.Log($"Chapter_Data에 데이터 삽입 실패 : {callback}");
            }

            // Chapter1_Hint 테이블들에 데이터 삽입
            for (int i = 0; i < 5; i++)
            {
                Param hintParam = new Param()
            {
                {$"chapterHint_{i+1}", data},
            };

                Backend.GameData.Insert($"Chapter{i + 1}_Hint", hintParam, hintCallback =>
                {
                    if (hintCallback.IsSuccess())
                    {
                        switch (i)
                        {
                            case 0:
                                chapterHintRowInData_1 = hintCallback.GetInDate();
                                break;
                            case 1:
                                chapterHintRowInData_2 = hintCallback.GetInDate();
                                break;
                            case 2:
                                chapterHintRowInData_3 = hintCallback.GetInDate();
                                break;
                            case 3:
                                chapterHintRowInData_4 = hintCallback.GetInDate();
                                break;
                            case 4:
                                chapterHintRowInData_5 = hintCallback.GetInDate();
                                break;
                        }
                        Debug.Log($"Chapter{i + 1}_Hint에 데이터 삽입 완료 : {hintCallback}");
                    }
                    else
                    {
                        Debug.Log($"Chapter{i + 1}_Hint에 데이터 삽입 실패 : {hintCallback}");
                    }
                });
            }

            // ChapterClear 테이블에 데이터 삽입
            Param clearParam = new Param()
        {
            {"chapterClear", data},
        };

            Backend.GameData.Insert("ChapterClear", clearParam, clearCallback =>
            {
                if (clearCallback.IsSuccess())
                {
                    chapterClearRowInData = clearCallback.GetInDate();
                    Debug.Log($"ChapterClear에 데이터 삽입 완료 : {clearCallback}");
                }
                else
                {
                    Debug.Log($"ChapterClear에 데이터 삽입 실패 : {clearCallback}");
                }
            });
        });
    }

    /// <summary>
    /// 뒤끝 콘솔 테이블에서 유저 정보를 불러올 때 호출
    /// </summary>
    public void GameDataLoad()
    {
        Backend.GameData.GetMyData("User_Data", new Where(), callback =>
        {
            //게임 정보 불러오기에 성공했을 때
            if (callback.IsSuccess())
            {
                Debug.Log($"게임 정보 데이터 불러오기 성공 : {callback}");
                //Json 데이터 파싱 성공
                try
                {
                    LitJson.JsonData gameDataJson = callback.FlattenRows();

                    //받아온 데이터의 개수가 0이면 데이터가 없는 것
                    if (gameDataJson.Count <= 0)
                    {
                        Debug.Log("데이터가 존재하지 않습니다");
                    }
                    else
                    {
                        //불러온 게임 정보의 고유값
                        gameDataRowInData = gameDataJson[0]["inDate"].ToString();

                        //불러온 게임의 정보를 userGameData 변수에 저장  
                        Manager.Data.UserGameData.localChartID = gameDataJson[0]["localChartID"].ToString();
                        Manager.Data.UserGameData.chapter = int.Parse(gameDataJson[0]["chapter"].ToString());
                        Manager.Data.UserGameData.hint = int.Parse(gameDataJson[0]["hint"].ToString());
                        Manager.Data.UserGameData.sfx = int.Parse(gameDataJson[0]["sfx"].ToString());
                        Manager.Data.UserGameData.bgm = int.Parse(gameDataJson[0]["bgm"].ToString());
                        Manager.Data.UserGameData.kor = bool.Parse(gameDataJson[0]["kor"].ToString());
                        Manager.Data.UserGameData.lobbyInfo = bool.Parse(gameDataJson[0]["lobbyInfo"].ToString());
                        Manager.Data.UserGameData.currentVer = gameDataJson[0]["currentVer"].ToString();

                        //유저 정보를 불러오는데 성공했을 때 호출할 메소드 지정
                        //onGameDataLoadEvent?.Invoke();
                    }
                }

                //Json 데이터 파싱 실패
                catch (System.Exception ex)
                {
                    //Utils.ShowInfo("Reset");
                    //유저정보를 초기값을 설정
                    Manager.Data.UserGameData.Reset();
                    //try-catch 에러 출력
                    Debug.Log(ex);
                }
            }
            //게임 정보 불러오기에 실패했을 때
            else
            {
                Debug.Log($"게임 정보 데이터 불러오기 실패 : {callback}");
            }
        });
    }

    /// <summary>
    /// 뒤끝 콘솔 테이블에서 챕터 정보를 불러올 때 호출
    /// </summary>
    /// <summary>
    /// 뒤끝 콘솔 테이블에서 챕터 정보를 불러올 때 호출
    /// </summary>
    public void ChapterDataLoad(Action<string, int> onLoad, int? hintNum = null)
    {
        string tableName;
        string dataKey;
        Action<LitJson.JsonData> dataHandler;
        string returnValue = default;

        if (!hintNum.HasValue)
        {
            // null일 때는 챕터 데이터를 불러옴
            tableName = "Chapter_Data";
            dataKey = "chapterData";
            dataHandler = gameDataJson =>
            {
                // 불러온 게임 정보의 고유값 저장
                chapterDataRowInData = gameDataJson[0]["inDate"].ToString();
                returnValue = gameDataJson[0][dataKey].ToString();
            };
        }
        else
        {
            // 0일 때는 클리어 데이터를 불러옴
            if (hintNum == 0)
            {
                tableName = "ChapterClear";
                dataKey = "chapterClear";
                dataHandler = gameDataJson =>
                {
                    chapterClearRowInData = gameDataJson[0]["inDate"].ToString();
                    returnValue = gameDataJson[0][dataKey].ToString();
                };
            }
            else
            {
                // 1~5일 때는 해당하는 챕터 힌트를 불러옴
                tableName = $"Chapter{hintNum}_Hint";
                dataKey = $"chapterHint_{hintNum}";
                dataHandler = gameDataJson =>
                {
                    switch (hintNum)
                    {
                        case 1:
                            chapterHintRowInData_1 = gameDataJson[0]["inDate"].ToString();
                            returnValue = gameDataJson[0][dataKey].ToString();
                            break;
                        case 2:
                            chapterHintRowInData_2 = gameDataJson[0]["inDate"].ToString();
                            returnValue = gameDataJson[0][dataKey].ToString();
                            break;
                        case 3:
                            chapterHintRowInData_3 = gameDataJson[0]["inDate"].ToString();
                            returnValue = gameDataJson[0][dataKey].ToString();
                            break;
                        case 4:
                            chapterHintRowInData_4 = gameDataJson[0]["inDate"].ToString();
                            returnValue = gameDataJson[0][dataKey].ToString();
                            break;
                        case 5:
                            chapterHintRowInData_5 = gameDataJson[0]["inDate"].ToString();
                            returnValue = gameDataJson[0][dataKey].ToString();
                            break;
                        default:
                            onLoad?.Invoke(null, -1);
                            return;
                    }
                };
            }
        }

        Backend.GameData.GetMyData(tableName, new Where(), callback =>
        {
            // 게임 정보 불러오기에 성공했을 때
            if (callback.IsSuccess())
            {
                Debug.Log($"{tableName} 데이터 불러오기 성공 : {callback}");

                LitJson.JsonData gameDataJson = callback.FlattenRows();

                // 받아온 데이터의 개수가 0이면 데이터가 없는 것
                if (gameDataJson.Count <= 0)
                {
                    Debug.Log("데이터가 존재하지 않습니다");
                    onLoad?.Invoke(null, -1);
                }
                else
                {
                    // 데이터 처리
                    try
                    {
                        dataHandler(gameDataJson);

                        // returnValue가 비어 있는지 확인
                        if (string.IsNullOrEmpty(returnValue))
                            onLoad?.Invoke(null, -1);
                        else
                            onLoad?.Invoke(returnValue, 1);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Custom Error {e.Message}");
                        onLoad?.Invoke(null, -1);
                    }
                }
            }
            // 게임 정보 불러오기에 실패했을 때
            else
            {
                Debug.Log($"{tableName} 데이터 불러오기 실패 : {callback}");
                onLoad?.Invoke(null, -1);
            }
        });
    }

    /// <summary>
    /// 뒤끝 콘솔에 있는 유저 데이터 갱신 
    /// </summary>
    public void GameDataUpdate(UnityAction action = null)
    {
        //유저 정보가 비어 있으면 함수를 종료한다.
        if (Manager.Data.UserGameData == null)
        {
            Debug.Log("서버에서 다운받거나 새로 삽입한 데이터가 존재하지 않습니다. Insert 혹은 Load를 통해 데이터를 생성해주세요");
            return;
        }

        Param param = new Param()
        {
            {"localChartID",    Manager.Data.UserGameData.localChartID},
            {"chapter",         Manager.Data.UserGameData.chapter},
            {"hint",            Manager.Data.UserGameData.hint},
            {"sfx",             Manager.Data.UserGameData.sfx},
            {"bgm",             Manager.Data.UserGameData.bgm},
            {"kor",             Manager.Data.UserGameData.kor},
            {"lobbyInfo",       Manager.Data.UserGameData.lobbyInfo},
            {"currentVer",      Manager.Data.UserGameData.currentVer},
        };

        //게임정보의 고유값(gameDataRowInData)이 없으면 에러메시지 출력
        if (string.IsNullOrEmpty(gameDataRowInData))
        {
            Debug.Log("유저의 inDate 정보가 없어 게임 정보 데이터 수정에 실패했습니다.");
        }

        //게임정보의 고유값이 있으면 테이블에 저장되어 있는 값 중 inDate 컬럼의 값과
        //소유하는 유저의 owner_inDate가 일치하는 row검색하여 수정하는 UpdataV2() 호출
        else
        {
            Debug.Log($"{gameDataRowInData} 게임 정보 데이터 수정을 요청합니다.");
            Backend.GameData.UpdateV2("User_Data", gameDataRowInData, Backend.UserInDate, param, callback =>
            {
                //데이터 수정에 성공했을 때
                if (callback.IsSuccess())
                {
                    Debug.Log($"게임 정보 데이터 수정에 성공했습니다.{callback}");
                    //매개변수로 받은 함수 실행
                    action?.Invoke();


                }

                //데이터 수정에 성공했을 때
                else
                {
                    Debug.Log($"게임 정보 데이터 수정에 실패했습니다.{callback}");
                }
            });
        }
    }

    /// <summary>
    /// 뒤끝 콘솔에 있는 챕터 데이터 갱신 
    /// </summary>
    public void ChapterDataUpdate(ChapterDataEnum chapter, string data)
    {
        Param param;
        string tableName = string.Empty;
        string inDate = string.Empty;

        switch (chapter)
        {
            case ChapterDataEnum.chapterData:
                param = new Param()
            {
                {"chapterData", data},
            };
                tableName = "Chapter_Data";
                inDate = chapterDataRowInData;
                break;

            case ChapterDataEnum.chapterHint_1:
                param = new Param()
            {
                {"chapterHint_1", data},
            };
                tableName = "Chapter1_Hint";
                inDate = chapterHintRowInData_1;
                break;

            case ChapterDataEnum.chapterHint_2:
                param = new Param()
            {
                {"chapterHint_2", data},
            };
                tableName = "Chapter2_Hint";
                inDate = chapterHintRowInData_2;
                break;

            case ChapterDataEnum.chapterHint_3:
                param = new Param()
            {
                {"chapterHint_3", data},
            };
                tableName = "Chapter3_Hint";
                inDate = chapterHintRowInData_3;
                break;

            case ChapterDataEnum.chapterHint_4:
                param = new Param()
            {
                {"chapterHint_4", data},
            };
                tableName = "Chapter4_Hint";
                inDate = chapterHintRowInData_4;
                break;

            case ChapterDataEnum.chapterHint_5:
                param = new Param()
            {
                {"chapterHint_5", data},
            };
                tableName = "Chapter5_Hint";
                inDate = chapterHintRowInData_5;
                break;

            case ChapterDataEnum.chapterClear:
                param = new Param()
            {
                {"chapterClear", data},
            };
                tableName = "ChapterClear";
                inDate = chapterClearRowInData;
                break;

            default:
                Debug.LogError("잘못된 ChapterDataEnum 값입니다.");
                return;
        }

        if (string.IsNullOrEmpty(inDate))
        {
            Debug.Log($"유저의 inDate 정보가 없어 {tableName} 데이터 수정에 실패했습니다.");
            return;
        }

        // 게임 정보의 고유값이 있으면 테이블에 저장된 데이터 수정 요청
        Debug.Log($"{inDate} 게임 정보 데이터 수정을 요청합니다.");
        Backend.GameData.UpdateV2(tableName, inDate, Backend.UserInDate, param);
    }


    public enum ChapterDataEnum
    {
        chapterData,
        chapterHint_1,
        chapterHint_2,
        chapterHint_3,
        chapterHint_4,
        chapterHint_5,
        chapterClear
    }



}
