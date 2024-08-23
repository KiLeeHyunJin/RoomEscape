using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class AssetBundleManager : Singleton<AssetBundleManager>
{
    //export?format=csv&gid=2146931206

    const string DownloadPath = "https://drive.google.com/uc?export=download&id=";
    private string serverVersionTableURL = "https://docs.google.com/spreadsheets/d/1pu9z0RT1m9YmvsiVd7uOAaL8JTutDJIVMhLrMT68RuI/export?format=csv&gid=2146931206";  // 서버 버전 테이블 접속 URL
    private string localVersionTablePath = "/versionTable.csv"; // 로컬 버전 테이블 경로
    private string localVersionPath = Application.streamingAssetsPath + "/AssetBundles";

    List<int> downLoadArray;
    List<string[]> serverVersionTable;
    List<string[]> localVersionTable;
    Queue<(UnityWebRequest, string)> waitQueue;

    bool localLoad;
    bool serverLoad;
    uint fetchSize;

    DownLoadUI downloadUI;
    [SerializeField] private DownLoadUI patchScreen;
    List<string> changeVersionBundleList;


    public List<string[]> VersionTable
    {
        get { return serverVersionTable; }
    }
    public void BundleVersionCheck(string bundleName, Action<bool> returnValue)
    {
        if (changeVersionBundleList == null)
        {
            StartCoroutine(LoadVersionTable(() =>
            {
                returnValue?.Invoke(changeVersionBundleList.Contains(bundleName));
            }));
        }
        else
        {
            changeVersionBundleList.Contains(bundleName);
        }
    }

    public void InitFetchCSV(Action call = null)
    {
        StartCoroutine(LoadVersionTable(call));
    }

    void ShowDownloadSizePopUp()
    {
        //디스크 잔여량 확인
        ulong freeSpace = GetFreeDiskSpace();
        if (freeSpace == 0)
        {
            Debug.LogError("용량 확인 불가 : Drive is not ready");
        }
        Debug.Log($"잔여량{FormatBytes(freeSpace)}");
        //다운로드 팝업창 생성

        downloadUI = Instantiate(patchScreen);

        //downloadUI.SetPopup(DownLoadUI.Popups.FindPopup);
        //downloadUI.FindFileCount.text = waitQueue.Count.ToString();
        //downloadUI.FindFileSize.text = fetchSize.ToString();
        SetMainLoadingText("곧 게임이 시작됩니다.");
        //downloadUI.StartDownloadAction(() => StartCoroutine(DownLoadStart(waitQueue, fetchSize)));
    }

    public void GetBundle(string url, string version, Action<AssetBundle> call = null)
    {
        StartCoroutine(GetBundleRoutine(url, version, call));
    }

    IEnumerator GetBundleRoutine(string url, string version, Action<AssetBundle> call)
    {
        UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(url, Hash128.Parse(version));
        yield return uwr.SendWebRequest();
        if (uwr.result == UnityWebRequest.Result.Success)
            call?.Invoke(DownloadHandlerAssetBundle.GetContent(uwr));
    }

    public void DownLoad(Action call, params string[] downloadFileName)
    {
        List<string> downloadFileNameList = new(downloadFileName);
        if (downLoadArray == null)
            downLoadArray = new(downloadFileNameList.Count);
        else
            downLoadArray.Clear();

        StartCoroutine(LoadAssetBundleProgress(downloadFileNameList, call)); // 패치 시작
    }


    /// <summary>
    /// 에셋 번들 패치 프로세스
    /// </summary>
    IEnumerator LoadAssetBundleProgress(List<string> downloadFileNameList, Action call)
    {
        //패치버전 체크
        yield return CheckBundleVersion(downloadFileNameList);
        //다운로드 체크
        yield return ParallerLoadAssetBundle(); // 병렬 방식 패치

        call?.Invoke();
        print($"done");
    }

    IEnumerator LoadVersionTable(Action call = null)
    {
        //로드를 하였고 패치 정보 확인을 하였기떄문에 종료
        if (localLoad && serverLoad)
        {
            call?.Invoke();
            yield break;
        }
        changeVersionBundleList ??= new();
        //serverLoad =    serverVersionTable == null;
        localLoad = localVersionTable == null;
        if (localLoad )
        {
            StartCoroutine(LoadLocallVersionTable());
        }

        using UnityWebRequest uwr = UnityWebRequest.Get(serverVersionTableURL);
        StartCoroutine(LoadServerVersionTable(uwr));

        while (localLoad == false || serverLoad == false)
        {
            Debug.Log("대기중");
            yield return null;
        }

        // 로컬 데이터가 없거나 서버와의 변경상황이 있다면
        bool isDownload = localVersionTable == null || VersionCheck();

        if (isDownload) //로컬 데이터가 없거나 다르다면
        {
            if (localVersionTable == null)
            {
                foreach (string[] serverData in serverVersionTable)
                    changeVersionBundleList.Add(serverData[(int)VersionTableColumn.FileName]);
            }

            System.IO.FileStream fs = new($"{localVersionPath}{localVersionTablePath}", System.IO.FileMode.Create);
            //파일 생성
            byte[] data = uwr.downloadHandler.data;
            fs.Write(data, 0, data.Length);
            fs.Dispose();
        }
        else //서버 데이터를 못가져왔다면
        {
            serverVersionTable ??= localVersionTable;
        }
        call?.Invoke();
    }

    bool VersionCheck()
    {
        //서버 데이터를 못가져왔다면 false
        if (serverVersionTable == null)
            return false;

        bool isDownload = false;
        bool isCheckChangeList = changeVersionBundleList.Count == 0;
        if (localVersionTable.Count == serverVersionTable.Count)
        {
            for (int i = 0; i < localVersionTable.Count; i++)
            {
                if (string.Equals(
                    serverVersionTable[i][(int)VersionTableColumn.Version],
                    localVersionTable[i][(int)VersionTableColumn.Version]) == false)
                {
                    string fileName = serverVersionTable[i][(int)VersionTableColumn.FileName];

                    if (isDownload == false)
                        isDownload = true;

                    if (isCheckChangeList)
                        changeVersionBundleList.Add(fileName);

                    Caching.ClearOtherCachedVersions(
                        fileName,
                        Hash128.Parse(serverVersionTable[i][(int)VersionTableColumn.Version]));
                }
            }
        }
        return isDownload;
    }

    IEnumerator ParallerLoadAssetBundle()
    {
        Debug.Log("Paraller");
        if (waitQueue == null)
            waitQueue = new();
        else
            waitQueue.Clear();
        fetchSize = default;
        SetMainLoadingText("패치를 진행하는 중");

        // 대기 큐 생성
        foreach (int idx in downLoadArray)
        {
            SetSubLoadingText($"패치를 진행하고 있습니다.");

            string version = VersionTable[idx][(int)VersionTableColumn.Version];
            string downloadURL = ExtractSubstring(VersionTable[idx][(int)VersionTableColumn.DownloadLink]); // 다운로드 링크로 재조합

            //존재하는지 확인
            if (Caching.IsVersionCached(downloadURL, Hash128.Parse(version)))
                continue;

            //존재하지 않는다면 다운로드 준비
            UnityWebRequest uwr = UnityWebRequest.Head(downloadURL);
            yield return uwr.SendWebRequest();
            string size = uwr.GetResponseHeader("Content-Length");
            uint.TryParse(size, out uint usize);
            fetchSize += usize;

            UnityWebRequest tupleUwr = UnityWebRequestAssetBundle.GetAssetBundle(downloadURL, Hash128.Parse(version), 0);
            (UnityWebRequest, string) tp = new(tupleUwr, VersionTable[idx][(int)VersionTableColumn.FileName]);

            waitQueue.Enqueue(tp);
        }

        if (waitQueue.Count == 0)
            yield break;
        StartCoroutine(DownLoadStart(waitQueue, fetchSize));
        //ShowDownloadSizePopUp();
    }

    IEnumerator DownLoadStart(Queue<(UnityWebRequest, string)> waitQueue, ulong fetchSize)
    {
        //다운로드 팝업 활성화
        //downloadUI.SetPopup(DownLoadUI.DownloadState.Downloading);
        int parallerSize = 4; // 병렬 동시 처리 요청 최대 개수 
        List<(UnityWebRequest, string)> progressList = new();

        StringBuilder progressPercentage = new();
        StringBuilder progressCount = new();
        int progressDoneCount = default;
        int bundleTotalCount = downLoadArray.Count; // 로드해야 할 번들 총 개수
        ulong currentSize;
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
                if (!progressList[i].Item1.isDone)
                {
                    progressPercentage.Append($"[{(int)(progressList[i].Item1.downloadProgress * 100)}%] - ");
                    percentage += progressList[i].Item1.downloadProgress;
                }
                else
                {
                    if (changeVersionBundleList.Contains(progressList[i].Item2))
                        changeVersionBundleList.Remove(progressList[i].Item2);
                    progressDoneCount++;
                    progressList.RemoveAt(i);
                }
            }

            currentSize = default;
            for (int i = 0; i < progressList.Count; i++)
            {
                currentSize += progressList[i].Item1.downloadedBytes;
            }

            //다운로드 현황 표기
            //downloadUI.DownloadPer.text = $"{(float)Math.Round(((percentage * 100) / bundleTotalCount), 1) }%";
            //downloadUI.DownloadSize.text = $"{FormatBytes(currentSize)} / {fetchSize}";
            Debug.Log($"{FormatBytes(currentSize)} / {fetchSize}");
            progressCount.Clear();
            progressPercentage.Clear();
            yield return new WaitForSeconds(0.1f);
        }
        //downloadUI.SetPopup(DownLoadUI.DownloadState.Complete);
        yield return null;
    }
    IEnumerator LoadServerVersionTable(UnityWebRequest uwr)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            serverLoad = true;
            yield break;
        }

        SetSubLoadingText("서버 버전 테이블을 조회합니다.");

        yield return uwr.SendWebRequest();

        string[] rows = uwr.downloadHandler.text.Split('\n');
        int length = rows.Length;
        serverVersionTable ??= new(length);

        for (int i = 0; i < length; i++)
            serverVersionTable.Add(rows[i].Split(','));
        serverLoad = true;
    }

    IEnumerator LoadLocallVersionTable()
    {
        if (System.IO.Directory.Exists(localVersionPath))
        {
            TextAsset localVersion = Resources.Load<TextAsset>($"{localVersionPath}{localVersionTablePath}");
            if (localVersion != null)
            {
                string[] rows = localVersion.text.Split('\n');
                int length = rows.Length;
                localVersionTable ??= new(length);

                for (int i = 0; i < length; i++)
                    localVersionTable.Add(rows[i].Split(','));
            }
        }
        else
        {
            System.IO.Directory.CreateDirectory(localVersionPath);
        }
        localLoad = true;
        yield return null;
    }

    /// <summary>
    /// 서버 버전 테이블 저장
    /// </summary>
    IEnumerator CheckBundleVersion(List<string> downloadFileNameList)
    {
        SetMainLoadingText("버전 정보를 받아오는 중");
        //csv를 가져왔는지 확인
        if (VersionTable == null)
            yield return StartCoroutine(LoadVersionTable());

        //테이블에서 다운로드가 필요한 목록 번호만 가져옴
        for (int i = 0; i < VersionTable.Count; i++)
        {
            if (downloadFileNameList.Contains(VersionTable[i][(int)VersionTableColumn.FileName]))
                downLoadArray.Add(i);
        }
    }

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
        string driveLetter = Path.GetPathRoot(Application.persistentDataPath); // 유니티의 persistentDataPath가 위치한 드라이브의 문자 가져오기
        DriveInfo drive = new(driveLetter);
        return drive.IsReady ? (ulong)drive.AvailableFreeSpace : 0;
    }

    string FormatBytes(ulong bytes)
    {
        const int scale = 1024;
        string[] orders = new string[] { "Gb", "Mb", "Kb", "Bytes" };
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

    void SetMainLoadingText(string msg)
    {
        //if (loadingText != null)
        //    loadingText.text = msg;
    }

    void SetSubLoadingText(string msg)
    {
        //if (loadingSubText != null)
        //    loadingSubText.text = msg;
    }

    IEnumerator DownLoadRoutine(UnityWebRequest uwr)
    {
        yield return uwr.SendWebRequest();
    }

    public enum VersionTableColumn // 버전 테이블 Column(열) 정보
    {
        FileName, // 번들 파일 명
        Version, // 번들 버전 정보
        DownloadLink, // 번들 설치 링크
        ManifestName,
        ManifestLink,
    }


}
