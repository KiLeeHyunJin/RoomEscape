using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
public class CanvasInitializor : MonoBehaviour
{
    [SerializeField] RectTransform viewPort;
    [SerializeField] int stageTimerValue;
    [SerializeField] public Transform FieldObj;

    [SerializeField] string[] labels;
    private void OnEnable()
    {
        //Manager.DownLoad.DownLoad(CallInit, labels);

        Manager.Game.SetField(FieldObj);
        Mask mask = viewPort == null ? GetComponentInChildren<Mask>() : viewPort?.GetComponent<Mask>();
        if (mask != null)
        {
            mask.enabled = true;
            mask.gameObject.GetOrAddComponent<ResolutionSizeSynchroToFieldImage>();
        }
        //CallInit();
    }

    void CallInit()
    {
        TextMeshProUGUI[] textMeshProUGUIs = GetComponentsInChildren<TextMeshProUGUI>();
        Image[] images = GetComponentsInChildren<Image>();
        AddResourceRefComponenet();

        //StartCoroutine(SetFontRoutine(textMeshProUGUIs));
        //StartCoroutine(SetImageRoutine(images));
    }

    void AddResourceRefComponenet()
    {
        foreach (Component comp in GetComponentsInChildren<Component>())
        {
            if(comp.GetComponent<TextMeshProUGUI>() != null || comp.GetComponent<Image>() != null)
            {
                comp.gameObject.GetOrAddComponent<ResourceRef>();
            }
        }
    }

}
