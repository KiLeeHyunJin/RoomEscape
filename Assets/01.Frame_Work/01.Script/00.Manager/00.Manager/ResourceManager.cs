using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System;
using TMPro;

public class ResourceManager : Singleton<ResourceManager>
{
    #region Resources 폴더 로드 함수
    private Dictionary<string, UnityEngine.Object> resources;

    public T Load<T>(string path) where T : UnityEngine.Object
    {
        string key = $"{path}_{typeof(T)}";

        if (resources.TryGetValue(key, out UnityEngine.Object obj))
        {
            return obj as T;
        }
        else
        {
            T resource = Resources.Load<T>(path);
            resources.Add(key, resource);
            return resource;
        }
    }
    #endregion

    #region AddressableData
    private Dictionary<string, AsyncOperationHandle>[] assetArray;
    private Dictionary<string, AsyncOperationHandle>[] dontReleaseArray;
    private Dictionary<string, AsyncOperationHandle<Sprite[]>> multiArray;
    private Dictionary<string, AsyncOperationHandle<Sprite[]>> multidontReleaseArray;

    struct StateData
    {
        public ResourceType type;
        public string fileName;
        public bool donRelease;
    }

    class WaitData
    {
        public WaitData(ResourceType _type, string _fileName, bool _donRelease, Action<UnityEngine.Object> _call)
        {
            state.type = _type;
            state.fileName = _fileName;
            state.donRelease = _donRelease;
            call = _call;
        }
        public StateData state;
        public Action<UnityEngine.Object> call;
    }

    private Dictionary<string, AssetBundle> BundleDic 
    { get { return bundleDic ??= new();} }  
    private Dictionary<string, AssetBundle> DonReleaseBundleDic 
    { get { return donReleaseBundleDic ??= new();}}

    #endregion AddressableData


    //다운로드 중인 번들
    private HashSet<string> loadBundleName; 


    //다운로드 대기 번들
    private Dictionary<string, List<WaitData>> waitBundleDic;

    //다운로드 된 번들
    private Dictionary<string, AssetBundle> bundleDic;
    private Dictionary<string, AssetBundle> donReleaseBundleDic;

    public TMP_FontAsset FontBold { get { return fontBold; } }
    public TMP_FontAsset FontLight { get { return fontLight; } }

    TMP_FontAsset fontBold;
    TMP_FontAsset fontLight;

    /// <summary>
    /// 로드 시도 중인 개수를 반환합니다.
    /// </summary>
    public int LoadCount
    {
        get
        {
            foreach (var item in waitBundleDic) //대기중인 번들의 이름을 순회
                SendWaitCall(item.Key); //해당 번들의 로드 이름을 호출
            //제거하고 남은 로드 시도 중인 개수 합산 후 반환
            return waitBundleDic.Count;
        }
    }
    protected override void Awake()
    {
        base.Awake();
        //로드되어있는 에셋 전부 언로드
        AssetBundle.UnloadAllAssetBundles(true);

        resources = new();
        loadBundleName = new();
        waitBundleDic = new();

        fontBold = Manager.Resource.Load<TMP_FontAsset>("Font/Maplestory Bold SDF");
        fontLight = Manager.Resource.Load<TMP_FontAsset>("Font/Maplestory Light SDF");
        //InitAddressable();
    }

    /// <summary>
    /// 대기 목록 초기화
    /// </summary>
    /// 
    public void ClearWaitList()
    {
        waitBundleDic.Clear();
        loadBundleName.Clear();
    }

    /// <summary>
    ///해당 번들 목록에 추가
    /// </summary>
    public void GetLoadBundle(string bundleName, AssetBundle bundle, bool donReleaseState = false)
    {
        Dictionary<string, AssetBundle> dic = GetDictionary(donReleaseState);
        if (dic.ContainsKey(bundleName))
            return;

        dic.Add(bundleName, bundle);
        SendWaitCall(bundleName);
    }

    /// <summary>
    /// 번들 로드 시도
    /// </summary>
    public void GetAsset(string bundleName, string fileName, ResourceType _type,Action<UnityEngine.Object> returnCall = null, bool compulsion = false, bool donReleaseState = false)
    {
        Dictionary<string, AssetBundle> dic = GetDictionary(donReleaseState);

        if (dic.ContainsKey(bundleName))
        {
            CallMethod(new(_type, fileName, donReleaseState, returnCall),bundleName);
            if (waitBundleDic.ContainsKey(bundleName)) //대기중인 콜백이 있다면 해당 콜백 진행
                SendWaitCall(bundleName);
            return;
        }

        if(loadBundleName.Add(bundleName) == false)
        {
            if (waitBundleDic.ContainsKey(bundleName) == false)
                waitBundleDic.Add(bundleName, new());

            waitBundleDic[bundleName].Add(new(_type, fileName, donReleaseState, returnCall));
            return;
        }
        else
        {
            waitBundleDic.Add(bundleName, new());
            waitBundleDic[bundleName].
                Add(new(_type, fileName, donReleaseState, returnCall));

            Action<string, Action<AssetBundle>> loadAction = compulsion ?
            Manager.DownLoadBundle.GetLoadOrDownloadBundle : Manager.DownLoadBundle.GetLoadBundle;

            loadAction?.Invoke(bundleName, (bundle) =>
            {
                if (bundle == null)
                {
                    waitBundleDic.Remove(bundleName);
                    loadBundleName.Remove(bundleName);
                    LoadFail(bundleName);
                }
                else
                {
                    dic.Add(bundleName, bundle);
                    SendWaitCall(bundleName);
                }
            });
            return;
        }
    }


    /// <summary>
    /// 대기목록에 있는 번들의 이름을 로드목록에서 검색 후 있으면 시도. 시도여부는 반환값
    /// </summary>
    void SendWaitCall(string bundleName)
    {
        if (waitBundleDic.ContainsKey(bundleName) == false)
            return;

        if (BundleDic.ContainsKey(bundleName) ||
           DonReleaseBundleDic.ContainsKey(bundleName))
        {
            List<WaitData> waitList = waitBundleDic[bundleName];

            //콜백 호출 메소드 실행전에 대기열에서 제거
            waitBundleDic.Remove(bundleName);
            loadBundleName.Remove(bundleName);

            foreach (WaitData waitData in waitList)
                CallMethod(waitData, bundleName);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void GetLoadData<TObject>(string bundleName,in StateData stateData, Action<UnityEngine.Object> bundleRecall ) where TObject : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(stateData.fileName))
            return;

        Dictionary<string, AssetBundle> dic = GetDictionary(stateData.donRelease);

        if (dic.TryGetValue(bundleName, out AssetBundle bundle) && 
            bundle.Contains(stateData.fileName))
        {
            bundleRecall?.Invoke(bundle.LoadAsset<TObject>(stateData.fileName)); //데이터 로드 콜백 함수 실행
        }
    }

    void CallMethod(WaitData waitdata, string bundleName)
    {

        switch (waitdata.state.type)
        {
            case ResourceType.Animation:
                break;
            case ResourceType.Sprite:
                GetLoadData<Sprite>
                    (bundleName, in waitdata.state, waitdata.call);
                break;
            case ResourceType.AudioClip:
                GetLoadData<AudioClip>
                    (bundleName, in waitdata.state, waitdata.call);
                break;
            case ResourceType.GameObject:
                GetLoadData<GameObject>
                    (bundleName, in waitdata.state, waitdata.call);
                break;
            case ResourceType.Scriptable:
                GetLoadData<ScriptableObject>
                   (bundleName, in waitdata.state, waitdata.call);
                break;
        }
    }

    Dictionary<string, AssetBundle> GetDictionary(bool donRelease) 
        =>  donRelease? DonReleaseBundleDic : BundleDic;

    void LoadFail(string bundleName) 
        => Message.LogError($"{bundleName}번들 Load Fail.");






    public void ReleaseBundle()
    {
        foreach (KeyValuePair<string, AssetBundle> bundle in BundleDic)
            bundle.Value.Unload(true);

        BundleDic.Clear();
    }

    private void OnDestroy()
    {
        foreach (AssetBundle bundle in AssetBundle.GetAllLoadedAssetBundles())
        {
            //Debug.Log($"Unload {bundle.name}");
            bundle.Unload(true);
        }
    }
    



#region Addressable

    void InitAddressable()
    {
#region Addressable
        assetArray = new Dictionary<string, AsyncOperationHandle>[(int)AssetType.END];
        dontReleaseArray = new Dictionary<string, AsyncOperationHandle>[(int)AssetType.END];
        multiArray = new Dictionary<string, AsyncOperationHandle<Sprite[]>>();
        multidontReleaseArray = new Dictionary<string, AsyncOperationHandle<Sprite[]>>();

        for (int i = 0; i < (int)AssetType.END; i++)
        {
            assetArray[i] = new Dictionary<string, AsyncOperationHandle>();
            dontReleaseArray[i] = new Dictionary<string, AsyncOperationHandle>();
        }
#endregion
    }

    //문자열을 이용한 에셋 로드
    void LoadHandle<TObject>(string path, string key, Action<AsyncOperationHandle<TObject>> call = null, bool dontRelease = false) where TObject : UnityEngine.Object
    {
        Addressables.LoadAssetAsync<TObject>(path).Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                //핸들을 딕셔너리에 추가
                AddHandle(key, handle, dontRelease);
                //액션 실행
                call?.Invoke(handle);
            }
        };
    }
    //레퍼런스를 이용한 에셋 로드
    void LoadHandle<TObject>(AssetReferenceT<TObject> path, string key, Action<AsyncOperationHandle<TObject>> call = null, bool dontRelease = false) where TObject : UnityEngine.Object
    {
        path.LoadAssetAsync<TObject>().Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                //핸들을 딕셔너리에 추가
                AddHandle(key, handle, dontRelease);
                //액션 실행
                call?.Invoke(handle);
            }
        };
    }
    void LoadHandleMultiSprite(AssetReferenceSprite path, string key, Action<AsyncOperationHandle<Sprite[]>> call = null, bool dontRelease = false)
    {
        path.LoadAssetAsync<Sprite[]>().Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                //핸들을 딕셔너리에 추가
                AddHandle(key, handle, dontRelease);
                //액션 실행
                call?.Invoke(handle);
            }
        };
    }
    void LoadHandleMultiSprite(string path, string key, Action<AsyncOperationHandle<Sprite[]>> call = null, bool dontRelease = false)
    {
        Addressables.LoadAssetAsync<Sprite[]>(path).Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                //핸들을 딕셔너리에 추가
                AddHandle(key, handle, dontRelease);
                //액션 실행
                call?.Invoke(handle);
            }
        };
    }

    void AddHandle(string key, AsyncOperationHandle<Sprite[]> handle, bool dontRelease = false)
    {
        //추가할 위치를 선택
        Dictionary<string, AsyncOperationHandle>[] listDic = dontRelease ? dontReleaseArray : assetArray;
        if (listDic[(int)CheckEnum<Sprite>()].ContainsKey(key))
            return;
        listDic[(int)CheckEnum<Sprite>()].Add(key, handle);
    }
    //핸들을 딕셔너리에 추가
    void AddHandle<TObject>(string key, AsyncOperationHandle<TObject> handle, bool dontRelease = false) where TObject : UnityEngine.Object
    {
        //추가할 위치를 선택
        Dictionary<string, AsyncOperationHandle>[] listDic = dontRelease ? dontReleaseArray : assetArray;
        if (listDic[(int)CheckEnum<TObject>()].ContainsKey(key))
            return;
        listDic[(int)CheckEnum<TObject>()].Add(key, handle);
    }
    void AddHandleMultiSprite(string key, AsyncOperationHandle<Sprite[]> handle, bool dontRelease = false)
    {
        //추가할 위치를 선택
        Dictionary<string, AsyncOperationHandle<Sprite[]>> listDic = dontRelease ? multidontReleaseArray : multiArray;
        if (listDic.ContainsKey(key))
            return;
        listDic.Add(key, handle);
    }


    //키를 통해 핸들을 액션을 통해 반환 또는 실행 없을경우 레퍼런스를 이용해 에셋 로드
    public bool GetHandle<TObject>(AssetReferenceT<TObject> path, string key , Action<AsyncOperationHandle<TObject>> call = null, bool dontRelease = false) where TObject : UnityEngine.Object
    {
        //잘못된 형식이면 종료
        AssetType objectType = CheckEnum<TObject>();
        if (objectType == AssetType.Other)
            return false;
        //확인할 목록 선택
        Dictionary<string, AsyncOperationHandle>[] checkArray = dontRelease ? dontReleaseArray : assetArray;
        if (string.IsNullOrEmpty(key))
        {
            LoadHandle(path, UnityEngine.Random.Range(0,float.MaxValue).ToString(), call, dontRelease);
            return false;
        }
        //존재하면 액션 실행
        if (checkArray[(int)objectType].ContainsKey(key))
        {
            call?.Invoke(checkArray[(int)objectType][key].Convert<TObject>());
            return true;
        }
        else
        {
            LoadHandle(path, key, call, dontRelease);
            return false;
        }
    }
    public bool GetHandleMultiSprite(AssetReferenceSprite path, string key, Action<AsyncOperationHandle<Sprite[]>> call = null, bool dontRelease = false)
    {
        //잘못된 형식이면 종료
        AssetType objectType = AssetType.Sprite;
        if (objectType == AssetType.Other)
            return false;
        //확인할 목록 선택
        Dictionary<string, AsyncOperationHandle<Sprite[]>> checkDic = dontRelease ? multidontReleaseArray : multiArray;
        if (string.IsNullOrEmpty(key))
        {
            LoadHandleMultiSprite(path, UnityEngine.Random.Range(0, float.MaxValue).ToString(), call, dontRelease);
            return false;
        }
        //존재하면 액션 실행
        if (checkDic.ContainsKey(key))
        {
            call?.Invoke(checkDic[key]);
            return true;
        }
        else
        {
            LoadHandleMultiSprite(path, key, call, dontRelease);
            return false;
        }
    }
    public bool GetHandleMultiSprite(string path, string key, Action<AsyncOperationHandle<Sprite[]>> call = null, bool dontRelease = false)
    {

        //확인할 목록 선택
        Dictionary<string, AsyncOperationHandle<Sprite[]>> checkArray = dontRelease ? multidontReleaseArray : multiArray;
        if (string.IsNullOrEmpty(key))
        {
            LoadHandleMultiSprite(path, UnityEngine.Random.Range(0, float.MaxValue).ToString(), call, dontRelease);
            return false;
        }
        else if (checkArray.ContainsKey(key))
        {
            call?.Invoke(checkArray[key]);
            return true;
        }
        else
        {
            LoadHandleMultiSprite(path, key, call, dontRelease);
            return false;
        }
    }

    //키를 통해 핸들을 액션을 통해 반환 또는 실행 없을경우 문자열을 이용해 에셋 로드
    public bool GetHandle<TObject>(string path, string key, Action<AsyncOperationHandle<TObject>> call = null,  bool dontRelease = false) where TObject : UnityEngine.Object
    {
        AssetType objectType = CheckEnum<TObject>();
        if (objectType == AssetType.Other)
            return false;
        //확인할 목록 선택
        Dictionary<string, AsyncOperationHandle>[] checkArray = dontRelease ? dontReleaseArray : assetArray;
        if(string.IsNullOrEmpty(key))
        {
            LoadHandle(path, UnityEngine.Random.Range(0, float.MaxValue).ToString(), call, dontRelease);
            return false;
        }
        else if (checkArray[(int)objectType].ContainsKey(key))
        {
            call?.Invoke(checkArray[(int)objectType][key].Convert<TObject>());
            return true;
        }
        else
        {
            LoadHandle(path, key, call, dontRelease);
            return false;
        }
    }

    public bool GetHandle<TObject>(string key, Action<AsyncOperationHandle<TObject>> call,bool dontRelease = false) where TObject : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(key))
            return false;
        AssetType objectType = CheckEnum<TObject>();
        if (objectType == AssetType.Other)
            return false;
        //확인할 목록 선택
        Dictionary<string, AsyncOperationHandle>[] checkArray = dontRelease ? dontReleaseArray : assetArray;
        //존재하는지 확인
        if (checkArray[(int)objectType].ContainsKey(key))
        {
            //유효성 검사
            if (checkArray[(int)objectType][key].IsValid())
            {
                call?.Invoke(checkArray[(int)objectType][key].Convert<TObject>());
                return true;
            }
        }
        return false;
    }

    void ReleaseHandle<TObject>(int idx, Dictionary<string, AsyncOperationHandle>[] checkDic) where TObject : UnityEngine.Object
    {
        foreach (KeyValuePair<string, AsyncOperationHandle> pair in checkDic[idx])
        {
            if (pair.Value.IsValid())
            {
#if UNITY_EDITOR
                Debug.Log($"Delete : {pair.Value.Result}");
#endif
                Addressables.Release(pair.Value.Convert<TObject>());
            }
        }
    }

    void ReleaseHandle(AssetType type, Dictionary<string, AsyncOperationHandle>[] checkDic)
    {
        switch (type)
        {
            case AssetType.GameObject:
                ReleaseHandle<GameObject>((int)type, checkDic);
                break;
            case AssetType.Material:
                ReleaseHandle<Material>((int)type, checkDic);
                break;
            case AssetType.AudioClip:
                ReleaseHandle<AudioClip>((int)type, checkDic);
                break;
            case AssetType.Sprite:
                ReleaseHandle<Sprite>((int)type, checkDic);
                break;
            case AssetType.Font:
                ReleaseHandle<Font>((int)type, checkDic);
                break;
            case AssetType.TMP_FontAsset:
                ReleaseHandle<TMP_FontAsset>((int)type, checkDic);
                break;
        }
        foreach (KeyValuePair<string,AsyncOperationHandle<Sprite[]>> pair in multiArray)
        {
            if (pair.Value.IsValid())
                Addressables.Release(pair.Value);
        }
    }

    public void ReleaseAsset()
    {
        for (int i = 0; i < assetArray.Length; i++)
        {
            ReleaseHandle((AssetType)i, assetArray);
        }
    }
#endregion

    AssetType CheckEnum<TObject>() where TObject : UnityEngine.Object
    {
        return typeof(TObject).Name switch
        {
            "GameObject" => AssetType.GameObject,
            "Sprite" => AssetType.Sprite,
            "Material" => AssetType.Material,
            "AudioClip" => AssetType.AudioClip,
            "TMP_FontAsset" => AssetType.TMP_FontAsset,
            "Font" => AssetType.Font,
            _ => AssetType.Other,
        };
    }

    public enum AssetType
    {
        GameObject,
        Material,
        AudioClip,
        Sprite,
        TMP_FontAsset,
        Font,
        Sprites,

        Other,
        END
    }
    public enum BundleType
    {
        chapter01       = 1 << 5,
        chapter02       = 1 << 6,
        chapter03       = 1 << 7,        

        END = 1 << 31
    }
}