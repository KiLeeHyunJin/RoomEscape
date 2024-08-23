using System;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildClearData : IPreprocessBuildWithReport 
{
    
    public int callbackOrder { get { return 0; } }


    //���� �� ���� ������ �ʱ�ȭ
    public void OnPreprocessBuild(BuildReport report)
    {
        //��� ����
        string path = Path.Combine(Application.streamingAssetsPath, "AssetBundles");

        //���� �� ��� ����
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        DirectoryInfo directoryInfo = new(path);
        //�ش� ����� ���ϵ� Ž��
        FileInfo[] files = directoryInfo.GetFiles();
        if (files.Length > 0)
        {
            //���� ���� ����
            foreach (FileInfo file in files)
                file.Delete();
            UnityEditor.EditorUtility.DisplayDialog("Clear Data", $"������ ������ �õ��Ͽ����ϴ�.\n", "�Ϸ�");
        }
    }
}
