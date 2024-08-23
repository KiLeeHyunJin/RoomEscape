using BackEnd;
using LitJson;
using System.Collections.Generic;
using UnityEngine;
using static UserGameData;

/// <summary>
/// 뒤끝 차트를 관리하는 클래스
/// </summary>
public static class BackendChartData
{
    public static Dictionary<int, LogChartData> logChart; // 데이터테이블의 로그메시지를 담는 딕셔너리
    public static string logChartID; // 로그차트의 ID를 저장할 변수

    public static Dictionary<int, string> loadPath; // LoadPath를 저장할 딕셔너리
    public static string loadPathID; // LoadPath의 ID를 저장할 변수

    public static string newVer; // 새로운 버전을 저장할 변수

    static BackendChartData()
    {
        // 초기화: 로그차트 및 LoadPath 딕셔너리를 생성
        logChart = new Dictionary<int, LogChartData>();
        loadPath = new Dictionary<int, string>();
    }

    /// <summary>
    /// 유저데이터 로드 / 인터넷 연결 상태일 경우 뒤끝에서 차트를 불러와 로컬에 저장 / 로컬에서 로그차트를 불러오기
    /// </summary>
    public static void LoadAllChart()
    {
        // 유저 데이터를 로드
        // Manager.Data.LoadData(DataManager.DataName.UserData);
        //Manager.Data.LoadData(DataManager.DataName.ChapterClear);
        BackendGameData.Instance.GameDataLoad();
        Manager.Data.LoadData();

        // 인터넷에 연결되어 있는 경우
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            LoadAllChartID(); // 모든 차트 ID를 불러옴
            SaveLogChart(); // 로그차트를 저장
        }

        LoadLogChart(); // 로컬에서 로그차트를 불러옴
    }

    /// <summary>
    /// 뒤끝 콘솔의 로그차트를 로컬에 저장하기
    /// </summary>
    public static void SaveLogChart()
    {
        LogChartVerCheck(); // 로그차트의 버전 확인
        if (newVer == Manager.Data.UserGameData.currentVer)
        {
            Debug.Log("버전이 같으므로 다운받지 않습니다.");
            return; // 버전이 동일하면 다운로드하지 않음
        }
        else
        {
            Debug.Log("버전이 달라져 차트를 업데이트합니다.");
        }

        Backend.Chart.GetOneChartAndSave(logChartID); // 로그차트를 뒤끝에서 가져와 저장

        Manager.Data.UserGameData.currentVer = newVer; // 유저 데이터의 버전을 업데이트
        Manager.Data.UserGameData.SetData(GameDataEnum.LocalChartID, logChartID); // 로컬 차트 ID 저장
    }

    /// <summary>
    /// 로컬의 로그차트 불러오기
    /// </summary>
    public static void LoadLogChart()
    {
        // 로컬에서 로그차트 데이터를 가져와 JSON으로 변환
        LitJson.JsonData chartJson = JsonMapper.ToObject(Backend.Chart.GetLocalChartData(Manager.Data.UserGameData.localChartID));

        var rows = chartJson["rows"]; // 행 데이터를 가져옴

        logChart.Clear(); // 기존의 로그차트 데이터를 초기화

        // 각 행 데이터를 반복 처리하여 로그차트에 추가
        for (int i = 0; i < rows.Count; ++i)
        {
            LogChartData newChart = new LogChartData();
            int ID = int.Parse(rows[i]["TableID"]["S"].ToString()); // TableID를 정수로 변환
            newChart.korean = rows[i]["ko"]["S"].ToString(); // 한국어 로그 메시지
            newChart.english = rows[i]["en"]["S"].ToString(); // 영어 로그 메시지

            logChart.Add(ID, newChart); // 로그차트에 추가
        }
    }

    /// <summary>
    /// 뒤끝 콘솔의 차트 ID 불러오기 /
    /// 차트파일이 변경될 때마다 스크립트에 ID를 입력하는 과정을 생략하기 위해, 차트명을 통해 적용된 차트파일 ID를 불러와서 사용 /
    /// 기획팀이 뒤끝 콘솔에서 엑셀파일만 갱신하여 바로 게임에 적용하기 위함
    /// </summary>
    public static void LoadAllChartID()
    {
        // 차트 목록을 뒤끝 서버에서 가져옴
        var bro = Backend.Chart.GetChartList();

        // 차트 이름과 파일 ID를 저장하기 위한 딕셔너리 생성
        Dictionary<string, string> chartInfoDictionary = new Dictionary<string, string>();

        // 가져온 차트 목록을 반복하여 처리
        foreach (JsonData chartData in bro.FlattenRows())
        {
            // 차트 이름 추출
            string chartName = chartData["chartName"].ToString();
            // 선택된 차트 파일 ID 추출
            string chartFileId = chartData["selectedChartFileId"].ToString();

            // 차트 이름과 파일 ID를 딕셔너리에 추가
            chartInfoDictionary.Add(chartName, chartFileId);
        }

        // 로그차트와 LoadPath의 ID를 딕셔너리에서 가져와 변수에 저장
        chartInfoDictionary.TryGetValue("로그차트(Test)", out logChartID);
        chartInfoDictionary.TryGetValue("LoadPath", out loadPathID);
    }

    /// <summary>
    /// 뒤끝 콘솔의 LoadPath 차트 조회하기
    /// </summary>
    public static void LoadPathChart()
    {
        // LoadPath 차트를 뒤끝에서 조회
        var bro = Backend.Chart.GetChartContents(loadPathID);

        // 실패했을 때
        if (!bro.IsSuccess())
        {
            Debug.LogError(bro.ToString());
            return;
        }

        // 성공했을 때
        LitJson.JsonData json = bro.FlattenRows();

        loadPath.Clear(); // 기존의 LoadPath 데이터를 초기화

        // 각 데이터 항목을 처리하여 LoadPath에 추가
        for (int i = 0; i < json.Count; i++)
        {
            int id = int.Parse(json[i]["TableID"].ToString()); // TableID를 정수로 변환
            string path = json[i]["Path"].ToString(); // Path를 문자열로 변환

            loadPath.Add(id, path); // LoadPath에 추가
        }
    }

    /// <summary>
    /// 뒤끝 콘솔의 LoadPath 차트를 조회하고 입력받은 int ID에 해당하는 string Path를 반환하는 함수
    /// </summary>
    public static string LoadPath(int id)
    {
        LoadPathChart(); // LoadPath 차트를 조회
        loadPath.TryGetValue(id, out string path); // ID에 해당하는 경로를 조회
        return path; // 경로 반환
    }

    /// <summary>
    /// 로그차트의 버전을 확인하는 함수
    /// </summary>
    public static void LogChartVerCheck()
    {
        // 로그차트를 뒤끝에서 조회
        var bro = Backend.Chart.GetChartContents(logChartID);

        // 실패했을 때
        if (!bro.IsSuccess())
        {
            Debug.LogError(bro.ToString());
            return;
        }

        // 성공했을 때
        LitJson.JsonData json = bro.FlattenRows();

        logChart.Clear(); // 기존의 로그차트 데이터를 초기화

        // 새 로그차트 데이터를 생성하여 추가
        LogChartData newChart = new LogChartData();
        int id = int.Parse(json[0]["TableID"].ToString()); // TableID를 정수로 변환
        newChart.korean = json[0]["ko"].ToString(); // 한국어 로그 메시지

        logChart.Add(id, newChart); // 로그차트에 추가

        // 로그차트의 한국어 메시지를 새 버전으로 설정
        logChart.TryGetValue(0, out LogChartData logChartData);
        newVer = logChartData.korean.ToString();
    }
}

[System.Serializable]
public class LogChartData
{
    public string korean; // 한국어 로그 메시지
    public string english; // 영어 로그 메시지
}
