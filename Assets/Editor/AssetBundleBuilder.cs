using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class AssetBundleBuilder
{
    [UnityEditor.MenuItem("MyTool/Cache/AllCacheClear")]
    public static void ClearCache()
    {
        AssetBundle.UnloadAllAssetBundles(true);
        UnityWebRequest.ClearCookieCache();
       
        bool state = Caching.ClearCache();
        UnityEditor.EditorUtility.DisplayDialog("Cache Clear", $"캐시 정리를 시도하였습니다.\n 결과 : {state}", "완료");
    }

    [UnityEditor.MenuItem("MyTool/Bundle/Build_Bundle")]
    public static void BuildBundle()
    {
        if (!Directory.Exists(Define.dir))
            Directory.CreateDirectory(Define.dir);

        DirectoryInfo directoryInfo = new(Define.dir);

        foreach (FileInfo file in directoryInfo.GetFiles())
            file.Delete();

        if (!Directory.Exists($"{Define.dir}{Define.android}"))
            Directory.CreateDirectory($"{Define.dir}{Define.android}");

        AssetBundleManifest manifest = UnityEditor.BuildPipeline.BuildAssetBundles(
            $"{Define.dir}{Define.android}", 
            UnityEditor.BuildAssetBundleOptions.None, 
            UnityEditor.BuildTarget.Android);

        if (manifest != null)
        {
            string[] allBundles = manifest.GetAllAssetBundles();
            WriteBundleTable(allBundles);
        }

        UnityEditor.EditorUtility.DisplayDialog("에셋 번들 빌드", "에셋 번들 빌드를 완료했습니다.", "완료");

        string backPath = Path.GetDirectoryName(Application.streamingAssetsPath);
        backPath = Path.GetDirectoryName(backPath);
        string openPath = $"{backPath}{@"\Bundle\Android"}";
        try
        {
            if (!Directory.Exists($"{openPath}"))
            {
                Message.LogError($"경로가 존재하지 않습니다: {openPath}");
                return;
            }

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = $"{openPath}",
                UseShellExecute = true,
                Verb = "open"
            });
        }
        catch (Exception ex)
        {
            Message.LogError($"폴더를 여는 도중 오류가 발생했습니다: {ex.Message}");
        }

        AssetBundle.UnloadAllAssetBundles(true);
    }

    static void WriteBundleTable(string[] allBundles)
    {
        BundleTable table = new();
        table.bundleTable = new List<BundleData>();
        for (int i = 0; i < allBundles.Length; i++)
        {
            BundleData data;
            string path = $"{Define.dir}{Define.window}/{allBundles[i]}";

            AssetBundle assetBundle = AssetBundle.LoadFromFile(path);

            data.resourceName = assetBundle.GetAllAssetNames();
            data.bundleName = assetBundle.name;
            data.bundlePath = path;

            table.bundleTable.Add(data);
        }
        File.WriteAllText($"{Define.dir}{Define.bundleTable}", JsonUtility.ToJson(table));
    }




    [UnityEditor.MenuItem("MyTool/Bundle/Refresh_BundleTable")]
    public static void ReWriteTable()
    {
        AssetBundle.UnloadAllAssetBundles(true);
        BundleTable bundleTable = new();

        List<string> bundleList = Extension.GetAssetBundleNames();
        bundleTable.bundleTable = new List<BundleData>(bundleList.Count);

        foreach (string bundleName in bundleList)
        {
            BundleData data = new();
            string path = $"{Define.dir}{Define.window}/{bundleName}";
            AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
            if (assetBundle == null)
                continue;
            string[] assetNames = assetBundle.GetAllAssetNames();
            data.resourceName = assetNames;
            data.bundlePath = path;
            data.bundleName = assetBundle.name;

            bundleTable.bundleTable.Add(data);
        }
        File.WriteAllText($"{Define.dir}{Define.bundleTable}", JsonUtility.ToJson(bundleTable));
        UnityEditor.EditorUtility.DisplayDialog("Refresh AssetBundleTable", "AssetBundle의 Table정보가 갱신되었습니다.", "완료");
    }
}
