using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SwipeController : MonoBehaviour
{
    [SerializeField] GameObject scrollbar;
    [SerializeField] float scroll_pos = 0;
    [SerializeField] float[] pos;
    Coroutine swipeCo;

    public void Init(int count)
    {
        pos = new float[count];
        Vector2 sizedelta = (GetComponentInParent<ScrollRect>().transform as RectTransform).sizeDelta;

        for (int i = 0; i < count; i++)
        {
            (transform.GetChild(i).transform as RectTransform).sizeDelta = sizedelta;
        }
        swipeCo = StartCoroutine(SwipeRoutine());
    }
    public void FocusOn()
    {
        this.ReStartCoroutine(SwipeRoutine(), ref swipeCo);
    }

    private void OnDisable()
    {
        if (swipeCo != null)
            StopCoroutine(swipeCo);
    }

    private void OnApplicationPause(bool pause)
    {
        if(pause == false)
        {
            swipeCo = StartCoroutine(SwipeRoutine());
        }
        else
        {
            if(swipeCo != null)
            StopCoroutine(swipeCo);
        }
    }
 
    IEnumerator SwipeRoutine()
    {
        while(true)
        {
            float distance = 1f / (pos.Length - 1f);
            for (int i = 0; i < pos.Length; i++)
            {
                pos[i] = distance * i;
            }
            if (Input.GetMouseButton(0))
            {
                scroll_pos = scrollbar.GetComponent<Scrollbar>().value;
            }
            else
            {
                for (int i = 0; i < pos.Length; i++)
                {
                    if (scroll_pos < pos[i] + distance / 2 && scroll_pos > pos[i] - (distance / 2))
                    {
                        scrollbar.GetComponent<Scrollbar>().value = Mathf.Lerp(scrollbar.GetComponent<Scrollbar>().value, pos[i], 0.1f);
                    }
                }
            }
            for (int i = 0; i < pos.Length; i++)
            {
                if (scroll_pos < pos[i] + distance / 2 && scroll_pos > pos[i] - (distance / 2))
                {
                    transform.GetChild(i).localScale = Vector2.Lerp(transform.GetChild(i).localScale, new Vector2(1f, 1f), 0.1f);
                    for (int a = 0; a < pos.Length; a++)
                    {
                        if (a != i)
                        {
                            transform.GetChild(a).localScale = Vector2.Lerp(transform.GetChild(a).localScale, new Vector2(0.8f, 0.8f), 0.1f);
                        }
                    }
                }
            }
            yield return null;
        }
    }

}
