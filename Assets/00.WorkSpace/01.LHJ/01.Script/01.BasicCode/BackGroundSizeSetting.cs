using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// é�� �ʵ��� ũ�� ������ �缳���Ѵ�.
/// </summary>
public class BackGroundSizeSetting : MonoBehaviour
{
    [SerializeField] CanvasScaler scaler;
    private void Start()
    {
        InitSize();
    }
    /// <summary>
    /// ���� ��ü�� ����� �缳���Ѵ�.
    /// </summary>
    public void InitSize()
    {
        StartCoroutine(SynchRatio());
    }
    /// <summary>
    /// 1������ ���� ���� �����Ѵ�.
    /// </summary>
    IEnumerator SynchRatio()
    {
        yield return new WaitForEndOfFrame();
        FlowSynch();
    }

    /// <summary>
    /// ���� ��ü ũ�� ���� �����Ѵ�.(é�� �ʵ�)
    /// </summary>
    void FlowSynch()
    {
        //�ڽ��� ���� ��� ����
        if (transform.childCount == 0)
            return;

        //ĵ������ ���� ��� ��������
        if (scaler == null)
            scaler = GetComponentInParent<CanvasScaler>();
        //����̽� ���� ���
        float deviceHeightRatio = 
            Screen.height / (scaler.transform as RectTransform).localScale.x;

        //
        foreach (Transform parent in transform)
        {
            //���̰� ��������
            RectTransform parentRect = parent as RectTransform;
            float height = parentRect.rect.height;

            //���� ���ų� �̻��� ��� �ѱ��.
            if (height <= 0)
                continue;
            
            //������ ����Ѵ�.
            float adjustmentValue = deviceHeightRatio / height;

            //���� ũ�⿡ ������ ���Ѵ�.
            parentRect.sizeDelta *= adjustmentValue;
            //ũ�� �ʱ�ȭ
            parentRect.localScale = Vector3.one;

            //�ڽĵ��� ��ġ �� ��ĳ�ϸ� ������ ���Ѵ�.
            foreach (Transform child in parent)
            {
                RectTransform childRect = child as RectTransform;
                childRect.anchoredPosition *= adjustmentValue;
                childRect.localScale *= adjustmentValue;
            }
        }
    }
}
