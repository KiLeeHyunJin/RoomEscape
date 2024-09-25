using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Audio;
using UnityEngine.Events;

public class DataManager : Singleton<DataManager>
{
    [SerializeField] private UserGameData userGameData = new();

    private string path;
    public string DataPath
    {
        get
        {
            if (string.IsNullOrEmpty(path))
                InitPath();
            return path;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        if (string.IsNullOrEmpty(path))
            InitPath();
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




}
