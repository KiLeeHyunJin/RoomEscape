using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;



public class AddressableManager : MonoBehaviour
{
    [SerializeField] AssetLabelReference defaultLabel;
    [SerializeField] AssetReferenceGameObject defaultObj;
    [SerializeField] AssetReferenceGameObject matObj;
    [SerializeField] AssetLabelReference matLabel;
    long patchSize;
    Dictionary<string, long> patchMap;
    static string Chapter02URL;
    static string Chapter01URL;
    static string ChapterCatalog;



    void Start()
    {
        Manager.DownLoadBundle.LoadToServerVersion((state) => 
        { 
            if(state)
            {
                StartCoroutine(Init());
            }
        });

    }

    IEnumerator Init()
    {
        string catalog;
        patchMap = new();

        Chapter01URL = Manager.DownLoadBundle.GetBundleURL("chapter01");
        Chapter02URL = Manager.DownLoadBundle.GetBundleURL("chapter02");

        catalog = Manager.DownLoadBundle.GetBundleURL("catalog");
        yield return DownloadCatalog(catalog);

        catalog = Manager.DownLoadBundle.GetBundleURL("catalogHash");
        yield return DownloadCatalog(catalog);

        var initHandle = Addressables.InitializeAsync();
        yield return initHandle;

        if (initHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Addressables initialized successfully.");
        }
        else
        {
            Debug.LogError("Failed to initialize Addressables.");
        }

        StartCoroutine(CheckUpdateFiles());

        StartCoroutine(PatchFiles());
    }

    private IEnumerator DownloadCatalog(string url)
    {
        using (var www = new UnityEngine.Networking.UnityWebRequest(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // 다운로드 성공 시 처리 로직 (예: 파일 저장)
                Debug.Log($"Successfully downloaded catalog from {url}");
            }
            else
            {
                Debug.LogError($"Failed to download catalog from {url}: {www.error}");
            }
        }
    }

    IEnumerator InitAddressable()
    {
        var init = Addressables.InitializeAsync();
        yield return init;
    }

    IEnumerator PatchFiles()
    {
        var labelse = new List<string>() { defaultLabel.labelString/*, matLabel.labelString*/ };
 
        foreach (string label in labelse)
        {
            var handle = Addressables.GetDownloadSizeAsync(label);
            yield return handle;
            if(handle.Result != 0)
            {
                StartCoroutine(DownLoadLabel(label));
            }
        }
        yield return CheckDownLoad();
    }
    IEnumerator DownLoadLabel(string label)
    {
        patchMap.Add(label, 0);

        var handle = Addressables.DownloadDependenciesAsync(label, false);
        while(handle.IsDone == false)
        {
            patchMap[label] = handle.GetDownloadStatus().DownloadedBytes;
            yield return new WaitForFixedUpdate();
        }
        patchMap[label] = handle.GetDownloadStatus().TotalBytes;
        Addressables.Release(handle);

    }
    IEnumerator CheckUpdateFiles()
    {
        var labelse = new List<string>() { defaultLabel.labelString/*, matLabel.labelString*/ };
        patchSize = default;
        foreach (var label in labelse)
        {
            var handle = Addressables.GetDownloadSizeAsync(label);
            yield return handle;
            patchSize += handle.Result;
        }
        if(patchSize > 0)
        {
            //다운로드가 있을 경우
        }
        else
        {
            //다운로드가 없을 경우(보유 중일 경우)
        }
    }

    IEnumerator CheckDownLoad()
    {
        long total = 0;
        while(true)
        {
            total += patchMap.Sum(tmp => tmp.Value);
            Message.Log(total.ToString());
            if(total == patchSize)
            {
                Message.Log("Complete");
                defaultObj.InstantiateAsync().Completed +=(oper)=> 
                {
                    if (oper.Result == null)
                        return;

                    GameObject obj = GameObject.Instantiate(oper.Result); ;
                    obj.name = "__1";
                    obj.transform.SetParent(this.transform);
                };
                patchMap.Clear();
                yield break; ;
            }
            else
            {
                Message.Log(total);
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
