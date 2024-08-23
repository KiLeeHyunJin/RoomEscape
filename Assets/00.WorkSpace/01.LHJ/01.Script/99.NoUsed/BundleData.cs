using System;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public struct BundleTable
{
    public List<BundleData> bundleTable;
}

[Serializable]
public struct BundleData
{
    public string bundleName;
    public string bundlePath;
    public string[] resourceName;
}
