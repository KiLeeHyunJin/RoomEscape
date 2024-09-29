using System;
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
            //WriteBundleTable(allBundles);
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

}
