using System.Collections.Generic;
using UnityEngine;

public static class Manager
{
    public static GameManager Game { get { return GameManager.Instance; } }
    public static DataManager Data { get { return DataManager.Instance; } }
    public static PoolManager Pool { get { return PoolManager.Instance; } }
    public static ResourceManager Resource { get { return ResourceManager.Instance; } }
    public static SceneManager Scene { get { return SceneManager.Instance; } }
    public static SoundManager Sound { get { return SoundManager.Instance; } }
    public static UIManager UI { get { return UIManager.Instance; } }
    public static BackendManager Backend { get { return BackendManager.Instance; }}
    public static InventoryManager Inventory { get { return InventoryManager.Instance; } }
    public static ChapterManager Chapter { get { return ChapterManager.Instance; ; } }
    public static TextManager Text { get { return TextManager.Instance;} }
    //public static AssetBundleManager AssetBundle { get { return AssetBundleManager.Instance; } }
    public static DownLoadBundleManager DownLoadBundle { get { return DownLoadBundleManager.Instance; } }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        GameManager.ReleaseInstance();
        DataManager.ReleaseInstance();
        PoolManager.ReleaseInstance();
        ResourceManager.ReleaseInstance();
        SceneManager.ReleaseInstance();
        SoundManager.ReleaseInstance();
        UIManager.ReleaseInstance();
        BackendManager.ReleaseInstance();
        InventoryManager.ReleaseInstance();
        ChapterManager.ReleaseInstance();
        DownLoadBundleManager.ReleaseInstance();

        ResourceManager.CreateInstance();
        GameManager.CreateInstance();
        DataManager.CreateInstance();
        PoolManager.CreateInstance();
        SceneManager.CreateInstance();
        SoundManager.CreateInstance();
        UIManager.CreateInstance();
        BackendManager.CreateInstance();
        InventoryManager.CreateInstance();
        ChapterManager.CreateInstance();
        TextManager.CreateInstance();
        DownLoadBundleManager.CreateInstance();
        //AssetBundleManager.CreateInstance();
    }
}
