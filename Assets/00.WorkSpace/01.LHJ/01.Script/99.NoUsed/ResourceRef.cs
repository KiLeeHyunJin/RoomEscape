using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static ResourceManager;
using static ResourceRef;

public struct BundleRefData
{
    [EnumFlags] public BundleType bundleType;
    public ResourceType resourceType;
    public bool compulsion;
    public bool isNotFoundBundle;
    public int idx;
}


public class ResourceRef : MonoBehaviour
{


    [SerializeField] string refFileName;
    [SerializeField] BundleRefData bundleData;

    [EnumFlags] [SerializeField] BundleType bundleType;
    [SerializeField] ResourceType resourceType;
    [SerializeField] bool compulsion;
    [SerializeField] bool isNotFoundBundle;
    [SerializeField] int idx;
    private void Start()
    {
        OnLoad();
    }


    public void OnLoad()
    {
        if (isNotFoundBundle || bundleType == 0)
            return;
        if(int.TryParse(bundleType.ToString(),out int temp))
            return;
        if (string.IsNullOrEmpty(refFileName) == false)
            Load();
    }


    public void Init(in BundleTable table, string prefabName, bool prefab)
    {
        refFileName = "";
        idx = -1;
        ResoureTypeInit();

        if (string.IsNullOrWhiteSpace(gameObject.name))
            return;

        string[] underbarSplit = gameObject.name.ToString().Split('_');
        string[] commaSplit = gameObject.name.ToString().Split(',');
        string[] atSplit = gameObject.name.ToString().Split('@');

        string objName = underbarSplit[0].Length > commaSplit[0].Length ?
            commaSplit[0] : underbarSplit[0];
        objName = objName.Length > atSplit[0].Length ?
            atSplit[0] : objName;

        switch (resourceType)
        {

            case ResourceType.Sprite:
                if (TryGetComponent<Image>(out Image img))
                {
                    if (img.sprite == null)
                    {
                        DestroyImmediate(GetComponent<ResourceRef>(), true);
                        gameObject.name = $"{objName}@Null";
                        return;
                    }
                    else
                    {
                        gameObject.name = $"{objName}@{img.sprite.name}";
                    }
                }
                break;
            case ResourceType.Animation:
                break;
            case ResourceType.AudioClip:
                break;
        }

        refFileName = gameObject.name.Split('@')[Define.EndIndex];
        bool ignoreState = false;
        foreach (string checkName in Define.ignoreRefName)
        {
            if (string.Equals(checkName, refFileName))
            {
                DestroyImmediate(GetComponent<ResourceRef>(), true);
                return;
            }
        }

        if (ignoreState == false)
        {
            string[] splitMulti = refFileName.Split('_');
            if (splitMulti.Length == 2)
            {
                if (string.Equals(splitMulti[0], "기본ui"))
                {
                    refFileName = "BasicUI";
                }
                else if(string.Equals(splitMulti[0],"아이콘아틀라스"))
                {
                    refFileName = "IconAtlas";
                }
                else
                {
                    refFileName = splitMulti[0];
                }
                if (int.TryParse(splitMulti[1], out int num))
                    idx = num;
            }
            else
            {
                refFileName = splitMulti[0];
            }
            string searchName = refFileName.ToLower();

            foreach (BundleData data in table.bundleTable)
            {
                foreach (string assetName in data.resourceName)
                {
                    string[] filePath = assetName.Split('/');
                    string[] fileNameSplit = filePath[Define.EndIndex].Split('.');
                    string fileName = fileNameSplit[^2];
                    if (string.Equals(fileName, searchName))
                    {
                        if (Enum.TryParse<ResourceManager.BundleType>(data.bundleName, out BundleType type))
                            bundleType |= type;
                        else
                        {
                            DestroyImmediate(GetComponent<ResourceRef>(), true);
                            return;
                        }
                    }
                }
            }
            if(bundleType == 0)
            {
                if (string.Equals(refFileName, "Null") == false)
                {
                    NotFound(prefabName, prefab);
                    return;
                }
            }
        }
        isNotFoundBundle = false;
    }

    void NotFound(string prefabName, bool prefab)
    {
        Debug.LogError($"'{prefabName}'의 {gameObject.name}에 '{resourceType}'-'{refFileName}'가 포함된 번들을 찾지 못하였습니다.  prefabState : {prefab}");

        if (prefab)
        {
            isNotFoundBundle = true;
        }
        DestroyImmediate(GetComponent<ResourceRef>(), true);
    }

    void ResoureTypeInit()
    {
        if (GetComponent<Image>() != null)
        {
            resourceType = ResourceType.Sprite;
        }
        else if (GetComponent<AudioController>() != null)
        {
            resourceType = ResourceType.AudioClip;
        }
        else
        {
            resourceType = 0;
        }
    }


    void Load()
    {
        switch (resourceType)
        {
            case ResourceType.Sprite:
                {
                    //if (idx >= 0)
                    //{
                    //    Manager.Resource.GetAssetMultiSprite(bundleType.ToString(), refFileName, (resource) =>
                    //    {
                    //        if (resource == null)
                    //        {
                    //            NoLoadAsset();
                    //            return;
                    //        }

                    //        Sprite sprite = (Sprite)resource[idx];
                    //        if (TryGetComponent<Image>(out Image img))
                    //            img.sprite = sprite;
                    //        else
                    //            Debug.Log(sprite.name);

                    //    }, compulsion);
                    //}
                    //else
                    //{
                    //    Manager.Resource.GetAsset(bundleType.ToString(), refFileName, resourceType, (resource) =>
                    //    {
                    //        if (resource == null)
                    //        {
                    //            NoLoadAsset();
                    //            return;
                    //        }
                    //        GetComponent<Image>().sprite = (Sprite)resource;
                    //    }, compulsion);
                    //}

                }
                break;
            case ResourceType.Animation:
                LoadData<Animation>((resource) =>
                {

                });
                break;
            case ResourceType.AudioClip:
                LoadData<AudioClip>((resource) =>
                {

                });
                break;
            default:
                break;
        }
    }

    void NoLoadAsset()
    {
        Debug.LogWarning($"{refFileName} is Not Found");
    }

    void LoadData<TObject>(Action<UnityEngine.Object> call,bool donReleaseState = false) where TObject : UnityEngine.Object
    {
        Manager.Resource.GetAsset(bundleType.ToString(), refFileName, resourceType, call, donReleaseState);
    }

}
public enum ResourceType
{
    Sprite,
    Animation,
    AudioClip,
    GameObject,
    Scriptable,
}
public class EnumFlagsAttribute : PropertyAttribute
{
    public EnumFlagsAttribute() { }
}
