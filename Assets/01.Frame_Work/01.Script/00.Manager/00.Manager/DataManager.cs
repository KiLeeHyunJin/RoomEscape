using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Audio;
using UnityEngine.Events;

public class DataManager : Singleton<DataManager>
{
    [SerializeField] private UserGameData userGameData = new();
    public UserGameData UserGameData { get { return userGameData; } }
    //챕터에 대한 클리어 여부를 갖는 구조체
    [SerializeField] ChapterGameData chapterData;
    public class jsonDataLoadEvent : UnityEvent { }
    public jsonDataLoadEvent onJsonDataLoadEvent = new();
    private string json;
    private string path;
    [SerializeField] AudioMixer audioMixer;
    public string DataPath
    {
        get
        {
            if (string.IsNullOrEmpty(path))
                InitPath();
            return path;
        }
    }
    //챕터 클리어 여부에 대한 인덱서
    public (bool? isCleared, bool? isFirst) this[int idx]
    {
        get
        {
            //idx가 0과 END번호 사이인지 확인
            if (idx > -1 && idx < chapterData.isCleared.Count)
                return (chapterData.isCleared[idx], chapterData.isFirst[idx]);
            else
                return (null, null);
        }
        private set
        {
            if (idx < chapterData.isCleared.Count)
            {
                if (value.isCleared.HasValue)
                    chapterData.isCleared[idx] = value.isCleared.Value;
                if (value.isFirst.HasValue)
                    chapterData.isFirst[idx] = value.isFirst.Value;
                SaveData(DataName.ChapterClear);
            }
        }
    }

    public void LoadData()
    {
        BackendGameData.Instance.ChapterDataLoad(LoadClearData, 0);
    }

    public void LoadClearData(string json, int trash)
    {
        chapterData = JsonUtility.FromJson<ChapterGameData>(json);
    }

    public void SetChapterData(int chapterNum, (bool? isCleared, bool? isFirst) setData)
    {
        if (chapterNum <= 0)
            return;
        this[--chapterNum] = setData;
    }

    public void SetClearAlbum(DataManager.GameDataName chapterName, int idx, bool result)
    {
        if (idx > Define.WrongImageStageCount)
        {
            Message.LogError($"SetClearAlbum Is Out Range - {chapterName}의 {idx}번째 state 접근 시도 (최대 범위 : {Define.WrongImageStageCount})");
            return;
        }
        if (chapterData.wrongImageData == null)
            chapterData.InitWrongImageData((int)DataManager.GameDataName.END);

        chapterData.wrongImageData[(int)chapterName][idx] = result;
    }

    public bool GetClearAlbumState(DataManager.GameDataName chapterName, int idx)
    {
        if (chapterData.wrongImageData == null)
            chapterData.InitWrongImageData((int)DataManager.GameDataName.END);

        bool? getStat = chapterData.wrongImageData[(int)chapterName][idx];
        if (getStat.HasValue == false)
        {
            Message.LogError($"GetClearAlbumState Is Not Has value - {chapterName}의 {idx}번째 state 접근 시도");
            return false;
        }
        return getStat.Value;
    }

    public bool[] GetClearAlbumState(DataManager.GameDataName chapterName)
    {
        bool[] stateResult = new bool[Define.WrongImageStageCount];
        WrongImageData searchClass = chapterData.wrongImageData[(int)chapterName];

        for (int i = 0; i < Define.WrongImageStageCount; i++)
        {
            stateResult[i] = searchClass[i].GetValueOrDefault();
        }
        return stateResult;
    }

    public int GetOpenAlbumCount()
    {
        int returnValue = default;
        for (int i = 0; i < chapterData.isCleared.Count; i++)
        {
            if (chapterData.isCleared[i])
                returnValue++;
        }
        return returnValue;
    }



    protected override void Awake()
    {
        base.Awake();
        if (string.IsNullOrEmpty(path))
            InitPath();
        chapterData.Init(); //배열과 선언
        audioMixer = Resources.Load<AudioMixer>("AudioMixer");
    }

    void InitPath()
    {
        // 안드로이드의 외부 저장소 경로 가져오기
#if UNITY_ANDROID && !UNITY_EDITOR
        //using AndroidJavaClass environment = new("android.os.Environment");
        //using AndroidJavaObject externalStorageDirectory = environment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory");
        //string externalStoragePath = externalStorageDirectory.Call<string>("getAbsolutePath");
        path = Path.Combine(Application.persistentDataPath, "AssetBundles");
        StartCoroutine(VerifyPermissionsAndCreateDirectory(path));
#else
        path = Path.Combine(Application.streamingAssetsPath, "AssetBundles");
        CreateDirectoryIfNeeded(path);
#endif
    }

    void CreateDirectoryIfNeeded(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }

    private void OnApplicationPause(bool pause)
    {
        if (string.Equals(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, "LoginScene"))
            return;

        if (pause)
        {
            SaveData(DataName.UserData);
            //SaveData(DataName.ChapterClear);
            SaveChapterClear();
        }
    }

    private void OnApplicationQuit()
    {
        if (string.Equals(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, "LoginScene"))
            return;

        SaveData(DataName.UserData);
        //SaveData(DataName.ChapterClear);
        SaveChapterClear();
    }


    IEnumerator VerifyPermissionsAndCreateDirectory(string path)
    {
        // 권한 확인 및 요청
        yield return StartCoroutine(VerifyPermissions());

        if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead) == false ||
            Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite) == false)
        {
            Application.Quit();
        }
        else
        {
            // 폴더 생성
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Debug.Log($"Creat Directory : {path}");
            }
            else
                Debug.Log($"Has Directory : {path}");
        }
    }

    public void RequestPermissions()
    {
        StartCoroutine(VerifyPermissions());
    }


    IEnumerator VerifyPermissions()
    {
        // 외부 저장소 읽기 및 쓰기 권한 요청
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead) ||
            !Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            yield return new WaitUntil(() =>
                Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead) &&
                Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite));
        }
    }

    Coroutine userCo;
    Coroutine chapterCo;


    public void SaveChapterClear()
    {
        //SaveData(DataName.ChapterClear);
        BackendGameData.Instance.ChapterDataUpdate(BackendGameData.ChapterDataEnum.chapterClear, JsonUtility.ToJson(chapterData, true));
    }


    void SaveData(DataName name)
    {
        Coroutine co = null;
        switch (name)
        {
            case DataName.UserData:
                BackendGameData.Instance.GameDataUpdate();
                //co = userCo;
                break;
            case DataName.ChapterClear:
                co = chapterCo;
                break;
        }
        if (co != null)
            StopCoroutine(co);
        co = StartCoroutine(SaveRoutine(name));
        Message.Log("저장");
    }


    IEnumerator VerifyingPermissions(string permission)
    {
        if (Permission.HasUserAuthorizedPermission(permission) == false)
        {
            Permission.RequestUserPermission(permission);
            yield return new WaitUntil(() => Permission.HasUserAuthorizedPermission(permission));
        }
    }
    IEnumerator SaveRoutine(DataName name)
    {
#if UNITY_EDITOR == false && PLATFORM_ANDROID
        yield return VerifyingPermissions(Define.WRITE);
#endif

        json = name switch
        {
            DataName.UserData => JsonUtility.ToJson(userGameData, true),
            DataName.ChapterClear => JsonUtility.ToJson(chapterData, true),
            _ => null,
        };

        string filePath = Path.Combine(DataPath, $"{name}.txt");
        File.WriteAllText(filePath, json);
        onJsonDataLoadEvent?.Invoke();
        yield return null;
    }

    IEnumerator LoadRoutine(DataName name)
    {
#if UNITY_EDITOR == false && PLATFORM_ANDROID
        yield return VerifyingPermissions(Define.READ);
#endif

        if (File.Exists($"{DataPath}/{name}.txt"))
        {

            string json = File.ReadAllText($"{DataPath}/{name}.txt");
            if (name == DataName.UserData)
            {
                userGameData = JsonUtility.FromJson<UserGameData>(json);
                //Utils.ShowInfo($"Find File {name} Chapter is {userGameData.chapter}");
            }
            else
            {
                chapterData.Init();
                chapterData = JsonUtility.FromJson<ChapterGameData>(json);

            }

            onJsonDataLoadEvent?.Invoke();
        }
        else
        {
            //Utils.ShowInfo("Not Find File");
            ResetData(name);
        }
        //Utils.ShowInfo(userGameData.chapter.ToString());
        yield return null;
    }

    public void UserDataAllSave()
    {
        SaveData(DataName.UserData);

        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            BackendGameData.Instance.GameDataUpdate();
        }
    }

    private void ResetData(DataName name)
    {
        switch (name)
        {
            case DataName.UserData:
                userGameData.Reset();
                UserDataAllSave();
                break;
                //case GameDataName.Chapter1:
                //    chapter_1.Reset();
                //    break;
                // 챕터 추가 예정
        }
    }

    public bool HasPermissions()
    {
#if PLATFORM_ANDROID && UNITY_EDITOR == false
        // 권한 확인
        AndroidJavaObject activity = GetUnityActivity();
        AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
        AndroidJavaObject packageManager = context.Call<AndroidJavaObject>("getPackageManager");
        string packageName = context.Call<string>("getPackageName");
        string checkPermission = "checkPermission";
        int writeResult = packageManager.Call<int>(checkPermission, Define.WRITE, packageName);
        int readResult = packageManager.Call<int>(checkPermission, Define.READ, packageName);
        return writeResult == 0 && readResult == 0;
#else
        return true;
#endif
    }

    public bool HasPermission(string permission)
    {
#if PLATFORM_ANDROID && UNITY_EDITOR == false
        // 권한 확인
        AndroidJavaObject activity = GetUnityActivity();
        AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
        AndroidJavaObject packageManager = context.Call<AndroidJavaObject>("getPackageManager");
        string packageName = context.Call<string>("getPackageName");
        int result = packageManager.Call<int>("checkPermission", permission, packageName);
        return result == 0;
#else
        return true;
#endif
    }

    AndroidJavaObject GetUnityActivity()
    {
        // 현재 유니티 활동 가져오기
        AndroidJavaClass unityPlayer = new("com.unity3d.player.UnityPlayer");
        return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    }

    public void OptionDataLoad()
    {
        Manager.Text._Iskr = Manager.Data.UserGameData.kor;
        audioMixer.SetFloat("BGM", Manager.Data.UserGameData.bgm);
        audioMixer.SetFloat("SFX", Manager.Data.UserGameData.sfx);
    }

    public enum DataName
    {
        UserData,
        ChapterClear,
    }

    public enum GameDataName
    {
        Chapter1,
        Chapter2,
        Chapter3,
        Chapter4,
        Chapter5,
        END
    }
}
