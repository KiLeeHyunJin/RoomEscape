using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Networking;

public class DownLoadBundleManager : Singleton<DownLoadBundleManager>
{
    const string DownloadPath = "https://drive.google.com/uc?export=download&id=";
    const string serverVersionTableURL = "https://docs.google.com/spreadsheets/d/1pu9z0RT1m9YmvsiVd7uOAaL8JTutDJIVMhLrMT68RuI/export?format=csv";  // 서버 버전 테이블 접속 URL

    const string localVersionTablePath = "/versionTable.csv"; // 로컬 버전 테이블 경로
    string LocalVersionPath { get { return Manager.Data.DataPath; } }

    Dictionary<string, string[]> localVersionTable;
    Dictionary<string, string[]> serverVersionTable;
    Dictionary<string, string[]> VersionTable 
    { 
        get 
        {
            if (serverVersionTable != null)
                return serverVersionTable;
            if (localVersionTable != null)
                return localVersionTable;
            return null;
        } 
    }

    PlayerBeforeCheckPopup instancePopupUI;
    [SerializeField] DownLoadUI downLoadUI;


    string titleText;
    string detailText;
    
    protected override void Awake()
    {
        base.Awake();
        StartCoroutine(CreateDirectory(LoadToServerVersion));
    }

    Coroutine checking;
    private void OnDestroy()
    {
        if (checking != null)
            StopCoroutine(checking);
    }
    private void Start()
    {
#if UNITY_EDITOR
        checking = StartCoroutine(CurrentLoadedBundle());
#endif
    }

    /// <summary>
    /// 현재 로드되어있는 번들 콘솔창에 출력
    /// </summary>
    IEnumerator CurrentLoadedBundle()
    {
        StringBuilder sb = new();

        while (true)
        {
            foreach (var item in AssetBundle.GetAllLoadedAssetBundles())
                sb.Append(item.name + ", ");

            string write = sb.ToString();
            if (string.IsNullOrEmpty(write) == false)
            {
                Message.Log(write);
                sb.Clear();
            }
            yield return new WaitForSeconds(10);
        }
    }

    /// <summary>
    /// 번들이 있으면 로드
    /// </summary>
    public void GetLoadBundle(string bundleName, Action<AssetBundle> bundleCall)
    {
        HasBundleCheck(bundleName, (state) =>
        {
            if (state)
            {
                UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(
                    ExtractSubstring(VersionTable[bundleName][(int)VersionTableColumn.DownloadLink]),
                    Hash128.Parse(VersionTable[bundleName][(int)VersionTableColumn.Version]));

                uwr.SendWebRequest().completed +=
                (operat) =>
                {
                    bundleCall?.Invoke(DownloadHandlerAssetBundle.GetContent(uwr));
                };
            }
            else
            {
                Message.Log("해당 번들이 없습니다.");
                bundleCall?.Invoke(null);
            }
        });
    }

    /// <summary>
    /// 번들이 있으면 로드하고 없으면 다운로드 후 로드
    /// </summary>
    public void GetLoadOrDownloadBundle(string bundleName, Action<AssetBundle> bundleCall)
    {
        //파일을 갖고있는지 확인
        HasBundleCheck(bundleName, (state) =>
        {
            if (state)
            {
                StartCoroutine(GetBundleRoutine(bundleName, bundleCall));
            }
            else
            {
                //버전에 포함되어있는지 확인
                IncludeCurrentTableVersionCheck(bundleName, (checkNameState) =>
                {
                    if (checkNameState)
                    {
                        //다운로드 확인
                        StartCoroutine(DownloadBundle(bundleName, bundleCall));
                    }
                    else
                    {
                        //버전 쳌
                        LoadToServerVersion((refreshState) =>
                        {
                            //포함 확인
                            IncludeCurrentTableVersionCheck(bundleName, (reCheckNameState) =>
                            {
                                if (reCheckNameState)
                                {
                                    //다운로드 확인
                                    StartCoroutine(DownloadBundle(bundleName, bundleCall));
                                }
                                else
                                {
                                    Message.LogError($"CompulsoryDownload Bundle 이름이 잘못됨 : {bundleName}");
                                }
                            });
                        });
                    }
                });
            }
        });
    }

    /// <summary>
    /// 다운로드 팝업창 출력
    /// </summary>
    void setText()
    {
        BackendChartData.logChart.TryGetValue(171, out LogChartData logChartData);
        if (logChartData != null)
        {
            if (Manager.Text._Iskr == true)
            {
                detailText = logChartData.korean;
            }
            else
            {
                detailText = logChartData.english;
            }
        }
        int datailTextNum = 0;
        switch (Manager.Chapter.chapter)
        {
            case 0:
                break;
            case 1:
                datailTextNum = 2000;
                break;
            case 2:
                datailTextNum = 3000;
                break;
            case 3:
                datailTextNum = 4000;
                break;
            case 4:
                datailTextNum = 5000;
                break;
            case 5:
                datailTextNum = 6000;
                break;
        }
        BackendChartData.logChart.TryGetValue(datailTextNum, out LogChartData logChartData2);
        if (logChartData != null)
        {
            if (Manager.Text._Iskr == true)
            {
                titleText = logChartData2.korean;
            }
            else
            {
                titleText = logChartData2.english;
            }
        }
        instancePopupUI.InitText(titleText, detailText);
    }
    void OnDownloadPopup(Queue<(UnityWebRequest, string)> waitQueue, List<string> dowloadList, uint fetchSize)
    {
        //디스크 잔여량 확인
        //ulong freeSpace = GetFreeDiskSpace();
        //if (freeSpace == 0)
        //{
        //    Message.LogError("용량 확인 불가 : Drive is not ready");
        //}
        //Message.Log($"잔여량{FormatBytes(freeSpace)}");

        //다운로드 팝업창 생성
        PopUpUI popUpUI = Manager.UI.ShowPopUpUI("CheckPopup");
        if (popUpUI == null)
            return;
        instancePopupUI = popUpUI as PlayerBeforeCheckPopup;
        //팝업창 초기화
        instancePopupUI.Init();
        setText();

        instancePopupUI.InitAction(
        () =>
        {
            //모든 팝업 삭제
            Manager.UI.ClearPopUpUI();
            if (downloadCo != null)
                StopCoroutine(downloadCo);

            downloadCo = StartCoroutine(DownLoadRoutine(waitQueue, dowloadList.Count, fetchSize));
            instancePopupUI = null;
        },
        () =>
        {
            Manager.UI.ClearPopUpUI();
            instancePopupUI = null;
        });
    }
    Coroutine downloadCo;

    /// <summary>
    /// 다운로드 중일 경우 다운로드 취소
    /// </summary>
    public void DownloadCancle()
    {
        if (downloadCo != null)
            StopCoroutine(downloadCo);
        if (Manager.Chapter.chapter == 0)
            return;

        string bundleName = $"chapter{string.Format("{0:D2}", Manager.Chapter.chapter)}";
        //번들 소유 확인
        HasBundleCheck(bundleName, (state) =>
        {
            Manager.Resource.ReleaseBundle();
            Caching.ClearAllCachedVersions(bundleName);
        });
    }

    /// <summary>
    /// 해당 번들이 다운로드되어있는지 확인 후 콜백
    /// </summary>
    public void HasBundleCheck(string bundleFileName, Action<bool> stateCall)
    {
        if (localVersionTable == null)
        {
            LoadToServerVersion((refreshState) =>
            {
                if (refreshState)
                {
                    CacheCheck(bundleFileName, stateCall);
                }
                else
                {
                    Message.LogError("버전 로드에서 실패함 : HasBundleCheck");
                    stateCall?.Invoke(false);
                }
            });
        }
        else
        {
            CacheCheck(bundleFileName, stateCall);
        }
    }

    /// <summary>
    /// 해당 번들이 있는지 확인. 있다면 버전 확인
    /// </summary>
    void CacheCheck(string bundleFileName, Action<bool> call)
    {
        if (localVersionTable.ContainsKey(bundleFileName))
        {
            call?.Invoke(Caching.IsVersionCached(
                       ExtractSubstring(VersionTable[bundleFileName][(int)VersionTableColumn.DownloadLink]),
                       Hash128.Parse(VersionTable[bundleFileName][(int)VersionTableColumn.Version])));
        }
        else
            call?.Invoke(false);
    }

    /// <summary>
    /// 번들을 가져온다.
    /// </summary>
    IEnumerator GetBundleRoutine(string bundleName, Action<AssetBundle> bundleCall)
    {
        UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(
            ExtractSubstring(VersionTable[bundleName][(int)VersionTableColumn.DownloadLink]),
            Hash128.Parse(VersionTable[bundleName][(int)VersionTableColumn.Version]));

        UnityWebRequestAsyncOperation oper = uwr.SendWebRequest();
        while (oper.isDone == false)
            yield return new WaitForSeconds(0.1f);
        if (uwr.result == UnityWebRequest.Result.Success)
        {
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);
            bundleCall?.Invoke(bundle);
        }
    }

    /// <summary>
    /// 해당 번들이 버전에 포함되어있는지 확인
    /// </summary>
    public void IncludeCurrentTableVersionCheck(string bundleName, Action<bool> call)
    {
        if (localVersionTable != null)
        {
            bool state = VersionTable.ContainsKey(bundleName);
            call?.Invoke(state);
            return;
        }
        LoadToServerVersion((refreshState) =>
        {
            bool state = VersionTable.ContainsKey(bundleName);
            if (refreshState)
                call?.Invoke(state);
            else
                Message.LogError("상태 이상 : IncludeCurrentVersionCheck");
        });
    }

    /// <summary>
    /// 자동 다운로드
    /// </summary>
    IEnumerator DownloadBundle(string bundleName, Action<AssetBundle> callBundle = null)
    {
        int idx = -1;
        for (int i = 0; i < VersionTable.Count; i++)
        {
            if (string.Equals(VersionTable[bundleName][(int)VersionTableColumn.FileName], bundleName))
            {
                idx = i;
                break;
            }
        }
        if (idx < 0)
        {
            callBundle?.Invoke(null);
            yield break;
        }
        //번들 url 재설정 및 요청
        UnityWebRequest uwr = UnityWebRequest.Head(
            ExtractSubstring(VersionTable[bundleName][(int)VersionTableColumn.DownloadLink]));
        yield return uwr.SendWebRequest();
        //번들 로드
        UnityWebRequest tupleUwr = UnityWebRequestAssetBundle.GetAssetBundle(
            ExtractSubstring(VersionTable[bundleName][(int)VersionTableColumn.DownloadLink]),
            Hash128.Parse(VersionTable[bundleName][(int)VersionTableColumn.Version]));

        yield return DownLoadRoutine(tupleUwr);

        if (tupleUwr.result == UnityWebRequest.Result.Success)
            callBundle?.Invoke(DownloadHandlerAssetBundle.GetContent(tupleUwr));
        else
            callBundle?.Invoke(null);
    }

    /// <summary>
    /// 번들 다운로드 팝업
    /// </summary>
    public void DownLoad(Action call, params string[] downloadFileName)
    {
        List<string> downloadFileNameList = new(downloadFileName);
        //로컬 테이블이 없다면 
        if (VersionTable == null)
        {
            LoadToServerVersion((refeshState) =>
            {
                if (refeshState)
                    DownLoader(downloadFileNameList, call);
                else
                {
                    Message.LogError("DownLoad _ VersionRefresh refresh Error");
                }
            });
        }
        else
        {
            DownLoader(downloadFileNameList, call);
        }
    }

    /// <summary>
    /// 다운로드 준비
    /// </summary>
    void DownLoader(List<string> downloadFileNameList, Action call)
    {
        WriteDownloadList(downloadFileNameList, out List<string> downloadList);
        //Utils.ShowInfo($"다운로드 개수 : {downloadList.Count}");
        StartCoroutine(DownLoadAssetBundle(call, downloadList));
    }

    /// <summary>
    /// 팝업창 다운로드 진행
    /// </summary>
    IEnumerator DownLoadRoutine(Queue<(UnityWebRequest, string)> waitQueue, int downloadCount, uint fetchSize)
    {
        foreach ((UnityWebRequest, string) pair in waitQueue)
            Caching.ClearCachedVersion(pair.Item2, Hash128.Parse(VersionTable[pair.Item2][(int)VersionTableColumn.Version]));

        //다운로드 팝업 활성화
        DownLoadUI _downloadUI = Manager.UI.ShowPopUpUI(downLoadUI);
        int parallerSize = 4; // 병렬 동시 처리 요청 최대 개수 
        List<(UnityWebRequest, string)> progressList = new();

        int progressDoneCount = default;
        ulong currentSize;
        ulong clearSize = default;
        int completeCount = default;

        float percentage;
        while (waitQueue.Count > 0 || progressList.Count > 0)
        {
            // 병렬 처리 요청 개수에 여유가 있다면 대기 큐에서 뽑아서 통신 시작
            if (progressList.Count < parallerSize && waitQueue.Count > 0)
            {
                var v = waitQueue.Dequeue();
                StartCoroutine(DownLoadRoutine(v.Item1));
                progressList.Add(v);
            }
            percentage = default;
            // 처리 진행 정보 확인 및 갱신
            for (int i = 0; i < progressList.Count; i++)
            {
                if (progressList[i].Item1.isDone)
                {
                    progressDoneCount++;
                    clearSize += progressList[i].Item1.downloadedBytes;
                    Manager.Resource.GetLoadBundle(progressList[i].Item2, DownloadHandlerAssetBundle.GetContent(progressList[i].Item1));
                    Message.Log($"DownloadClear : {progressList[i].Item2}");
                    progressList.RemoveAt(i);
                    completeCount++;
                }
                else
                {
                    percentage += progressList[i].Item1.downloadProgress;
                }
            }
            currentSize = clearSize;
            for (int i = 0; i < progressList.Count; i++)
            {
                currentSize += progressList[i].Item1.downloadedBytes;
            }
            _downloadUI.GetStateValue((percentage + (float)completeCount) / (float)downloadCount);
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSecondsRealtime(1);
        _downloadUI.Complete();
    }

    /// <summary>
    /// 번들을 소유하고 있는지 확인
    /// </summary>
    public void GetHasBundleCheck(string bundleName, Action<bool> checkValueCall)
    {
        if (VersionTable == null)
        {
            LoadToServerVersion((refreshState) =>
            {
                if (refreshState)
                {
                    checkValueCall?.Invoke(CheckVersionCached(bundleName));
                }
                else
                {
                    Message.Log("로드 실패");
                    checkValueCall?.Invoke(false);
                }
            });
        }
        else
        {
            checkValueCall?.Invoke(CheckVersionCached(bundleName));
        }
    }

    /// <summary>
    /// 해당 번들의 버전 확인
    /// </summary>
    bool CheckVersionCached(string bundleName)
    {
        return VersionTable.ContainsKey(bundleName) &&
            Caching.IsVersionCached(
                    ExtractSubstring(VersionTable[bundleName][(int)VersionTableColumn.DownloadLink]),
                    Hash128.Parse(VersionTable[bundleName][(int)VersionTableColumn.Version]));
    }

    /// <summary>
    /// 다운로드 데이터 준비 및 다운로드 목록이 있으면 팝업창 출력
    /// </summary>
    IEnumerator DownLoadAssetBundle(Action call, List<string> downLoadArray)
    {
        Queue<(UnityWebRequest, string)> waitQueue = new();
        uint fetchSize = default;

        // 대기 큐 생성
        foreach (string bundleName in downLoadArray)
        {
            UnityWebRequest uwr = UnityWebRequest.Get(VersionTable[bundleName][(int)VersionTableColumn.DownloadLink]);
            UnityWebRequestAsyncOperation oper = uwr.SendWebRequest();
            oper.completed += (asyncOperation) =>
            {
                UnityWebRequest tupleUwr = UnityWebRequestAssetBundle.GetAssetBundle(
                    ExtractSubstring(VersionTable[bundleName][(int)VersionTableColumn.DownloadLink]),
                    Hash128.Parse(VersionTable[bundleName][(int)VersionTableColumn.Version]),
                    0);
                waitQueue.Enqueue(new(tupleUwr, bundleName));
            };
        }

        while (waitQueue.Count != downLoadArray.Count)
            yield return new WaitForSeconds(0.1f);

        if (waitQueue.Count > 0)
            OnDownloadPopup(waitQueue, downLoadArray, fetchSize);

        call?.Invoke();
    }

    /// <summary>
    /// 다운로드 목차 작성
    /// </summary>
    void WriteDownloadList(List<string> downloadBundleNameList, out List<string> downLoadArray)
    {
        downLoadArray = new();
        int checkCount = 0;
        if (downloadBundleNameList.Count == 0)
            return;

        foreach (string downloadBundleName in downloadBundleNameList)
        {
            if (VersionTable.ContainsKey(downloadBundleName))
            {
                checkCount++;
                //존재하는지 확인
                if (Caching.IsVersionCached(
                    ExtractSubstring(VersionTable[downloadBundleName][(int)VersionTableColumn.DownloadLink]),
                    Hash128.Parse(VersionTable[downloadBundleName][(int)VersionTableColumn.Version])) == false)
                {
                    downLoadArray.Add(downloadBundleName);
                }
            }
        }

        if (downloadBundleNameList.Count != checkCount)
            Message.LogWarning($"WriteDownloadList _ Not Match Download List Count Check {downloadBundleNameList.Count} / {checkCount}");
        else
            Message.Log($"CheckCount : {downloadBundleNameList.Count} Download Count : {downLoadArray.Count}");
    }



    #region VersionTable

    /// <summary>
    /// 번들 버전 최신화
    /// </summary>
    public void LoadToServerVersion(Action<bool> refeshState = null, Action downloadAction = null)
    {
        //if (VersionTable == null)
        //    LoadLocallVersionTable();

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Utils.ShowInfo("인터넷 끊김 ㄷㄷ");
            if (VersionTable == null)
            {
                Message.LogError("파일도 안받았는데 인터넷도 끊김 ㄷㄷ");
                refeshState?.Invoke(false);
                return;
            }
        }
        StartCoroutine(LoadServerVersionTable(refeshState, downloadAction));
    }



    //버전 비교
    //bool VersionCheck()
    //{
    //    bool isDownload = false;
    //    //if (VersionTable.Count == serverVersionTable.Count)
    //    {
    //        foreach (KeyValuePair<string, string[]> bundleVersion in VersionTable)
    //        {
    //            if (string.Equals(
    //              bundleVersion.Value[(int)VersionTableColumn.Version],
    //              bundleVersion.Value[(int)VersionTableColumn.Version]) == false)
    //            {
    //                string fileName = bundleVersion.Value[(int)VersionTableColumn.FileName];

    //                if (isDownload == false)
    //                    isDownload = true;

    //                //////변경된 번들 삭제
    //                //Caching.ClearOtherCachedVersions(
    //                //    ExtractSubstring(bundleVersion[bun]),
    //                //    Hash128.Parse(bundleVersion.Value[(int)VersionTableColumn.Version]));
    //            }
    //        }

    //    }
    //    return isDownload;
    //}

    /// <summary>
    /// 로컬 데이터 불러오기
    /// </summary>
    void LoadToLocalVersion()
    {
        //Utils.ShowInfo("LoadLocallVersionTable");
        if (System.IO.Directory.Exists($"{LocalVersionPath}{localVersionTablePath}"))
        {
            string localVersion = System.IO.File.ReadAllText($"{LocalVersionPath}{localVersionTablePath}");
            if (localVersion != null)
            {
                string[] rows = localVersion.Split('\n');
                int length = rows.Length;

                if (localVersionTable == null)
                    localVersionTable = new(length);
                else
                    localVersionTable.Clear();

                for (int i = 0; i < length; i++)
                {
                    string[] versionData = rows[i].Split(',');
                    localVersionTable.Add(versionData[(int)VersionTableColumn.FileName], versionData);
                }
            }
        }
        else
        {
            localVersionTable = null;
            Message.Log("로컬 데이터가 없습니다.");
        }
    }
    //서버 데이터 불러오기 
    IEnumerator LoadServerVersionTable(Action<bool> refeshState, Action call)
    {
        using UnityWebRequest uwr = UnityWebRequest.Get(serverVersionTableURL);
        yield return uwr.SendWebRequest();
       
        string[] rows = uwr.downloadHandler.text.Split('\n');
        int length = rows.Length;

        if (serverVersionTable == null)
            serverVersionTable = new();
        else
            serverVersionTable.Clear();

        for (int i = 0; i < length; i++)
        {
            string[] versionData = rows[i].Split(',');
            serverVersionTable.Add(versionData[(int)VersionTableColumn.FileName], versionData);
        }


        refeshState?.Invoke(serverVersionTable != null);
        call?.Invoke();
    }

    #endregion VersionTable

    #region NoDownloadBundleCount
    //로컬 버전 중 다운로드 되어있지 않은 번들 개수 반환
    public void NoDownloadBundleCount(Action<int> call)
    {
        if (localVersionTable == null)
        {
            LoadToServerVersion((versionState) =>
            {
                if (versionState == false)
                {
                    Message.LogError("버전 로드에서 실패함 : NoDownloadBundleCount");
                    call?.Invoke(-1);
                }
                else
                {
                    call?.Invoke(CheckDifferentVersionCount());
                }
            });
            return;
        }
        call?.Invoke(CheckDifferentVersionCount());
    }

    int CheckDifferentVersionCount()
    {
        List<string> downloadBundleList = new(VersionTable.Count);
        foreach (KeyValuePair<string, string[]> bundleVersionData in VersionTable)
        {
            if (Caching.IsVersionCached(
                ExtractSubstring(bundleVersionData.Value[(int)VersionTableColumn.DownloadLink]),
                Hash128.Parse(bundleVersionData.Value[(int)VersionTableColumn.Version])) == false)
            {
                downloadBundleList.Add(bundleVersionData.Value[(int)VersionTableColumn.FileName]);
            }
        }
        return downloadBundleList.Count;
    }
    #endregion NoDownloadBundleCount

    #region Tools


    string ExtractSubstring(string inputString)
    {
        const string startMarker = "d/";
        const string endMarker = "/view";

        int startIndex = inputString.IndexOf(startMarker);
        if (startIndex == -1)
            return null;
        startIndex += startMarker.Length;
        int endIndex = inputString.IndexOf(endMarker, startIndex);
        if (endIndex == -1)
            return null;

        return $"{DownloadPath}{inputString[startIndex..endIndex]}";
    }

    ulong GetFreeDiskSpace()
    {
        string driveLetter = Path.GetPathRoot(LocalVersionPath); // 유니티의 persistentDataPath가 위치한 드라이브의 문자 가져오기
        DriveInfo drive = new(driveLetter);
        return drive.IsReady ? (ulong)drive.AvailableFreeSpace : 0;
    }

    string FormatBytes(ulong bytes)
    {
        const int scale = 1024;
        string[] orders = new string[] { "GB", "MB", "KB", "Bytes" };
        ulong max = (ulong)Mathf.Pow(scale, orders.Length - 1);
        foreach (string order in orders)
        {
            if (bytes > max)
            {
                return string.Format("{0:##.##} {1}", decimal.Divide(bytes, max), order);
            }
            max /= scale;
        }
        return "0 Bytes";
    }
    IEnumerator DownLoadRoutine(UnityWebRequest uwr)
    {
        yield return uwr.SendWebRequest();
    }
    #endregion Tools

    /// <summary>
    /// 권한이 없을 경우 권한 요청 , 경로 없을 경우 경로 생성
    /// </summary>
    private System.Collections.IEnumerator CreateDirectory(Action<Action<bool>, Action> call)
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }

        // 권한 요청 결과를 기다림
        while (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            yield return null;
        }

        // 디렉토리가 존재하지 않는다면 생성
        //if (!Directory.Exists(LocalVersionPath))
        //{
        //    Directory.CreateDirectory(LocalVersionPath);
        //}
        call?.Invoke(null, null);
    }


    public enum VersionTableColumn // 버전 테이블 Column(열) 정보
    {
        FileName, // 번들 파일 명
        Version, // 번들 버전 정보
        DownloadLink, // 번들 설치 링크
    }
}
