using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ŭ�� ����Ʈ Ŭ����
/// </summary>
public class ClickEffectRatio : PooledObject
{
    [SerializeField] float multipleScale;
    [SerializeField] float duringTime;

    RectTransform rectTransform;
    Image img;

    private void Awake()
    {
        //���� ��ü�� ��ƮƮ�������� �����´�.
        rectTransform = GetComponent<RectTransform>();
        //���� ��ü�� �̹����� �����´�.
        img = GetComponent<Image>();
    }

    /// <summary>
    /// �Ű����� ��ġ�� ���� ���ӿ�����Ʈ�� ��ġ�Ѵ�.
    /// </summary>
    public void SetPosition(Vector2 pos)
    {
        rectTransform.anchoredPosition = pos;
    }

    /// <summary>
    /// ���� ������ �ʱ�ȭ�Ѵ�.
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();
        //ũ���� ������ 1����� �����Ѵ�.
        rectTransform.localScale = Vector3.one;
        //���� �̹����� ���� ������ �����´�.
        Color color = img.color;
        //Alpha���� 1�� �����Ѵ�.(�ִ밪)
        color.a = 1;
        //������ ���� ������ �����ؼ� �����Ѵ�.
        img.color = color;
        //Ŭ�� ����Ʈ ��ƾ�� �����Ѵ�.
        StartCoroutine(Effect());
    }

    /// <summary>
    /// Ŭ�� ����Ʈ ��ƾ
    /// </summary>
    IEnumerator Effect()
    {
        while (true)
        {
            //   (1ƽ �����ð� / �����ð�)  ���� �ð� ������ ����Ѵ�.
            float timeScale = (Time.deltaTime / duringTime);
            // (1�ʰ� ����� �������� ���� * ���� �ð� ���� )�� ���� �����Ͽ� ���Ѵ�.
            rectTransform.localScale += new Vector3(multipleScale, multipleScale, 0) * timeScale;
            //Alpha�� ������ ���� �ð� ������ŭ �����Ѵ�.
            Color color = img.color;
            color.a -= timeScale;
            img.color = color;
            yield return null;
            //Alpha���� 0.02 ������ ��� ��ƾ�� �����Ѵ�. 
            if (color.a <= 0.02f)
                break;
        }
        //��ü�� ������Ʈ Ǯ�� ��ȯ�Ѵ�.
        Release();
    }
}
