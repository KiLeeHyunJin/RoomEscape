using System;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildClearData : IPreprocessBuildWithReport 
{
    
    public int callbackOrder { get { return 0; } }


    //빌드 시 저장 데이터 초기화
    public void OnPreprocessBuild(BuildReport report)
    {
        //경로 생성
        string path = Path.Combine(Application.streamingAssetsPath, "AssetBundles");

        //없을 시 경로 생성
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        DirectoryInfo directoryInfo = new(path);
        //해당 경로의 파일들 탐색
        FileInfo[] files = directoryInfo.GetFiles();
        if (files.Length > 0)
        {
            //파일 전부 삭제
            foreach (FileInfo file in files)
                file.Delete();
            UnityEditor.EditorUtility.DisplayDialog("Clear Data", $"데이터 삭제를 시도하였습니다.\n", "완료");
        }
    }
}
