
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SliderLock : PopUpUI
{
    [SerializeField] Dictionary<int, SliderCircle> circles;
    [SerializeField] List<SliderCircle> lines;
    [SerializeField] Canvas canvas;
    [SerializeField] GameObject grid;
    [SerializeField] GameObject linePrefab;
    [SerializeField] GameObject lineOnEdit;
    [SerializeField] RectTransform lineOnEditRT;
    [SerializeField] SliderCircle circleOnEdit;
    [SerializeField] bool unLocking;
    [SerializeField] bool isEnabled = true;
    [SerializeField] List<int> correctPattern; // 정답 패턴을 저장하는 리스트
    private List<int> currentPattern = new List<int>(); // 사용자가 입력한 패턴을 저장하는 리스트
    [SerializeField] GameObject clearMessage;
    [SerializeField] bool changeQuestion;
    [SerializeField] int questionNum;

    protected override void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        circles = new Dictionary<int, SliderCircle>();

        for (int i = 0; i < grid.transform.childCount; i++)
        {
            var circle = grid.transform.GetChild(i);

            var sliderCircle = circle.GetComponent<SliderCircle>();

            sliderCircle.id = i;
            sliderCircle.sliderLock = this;

            circles.Add(i, sliderCircle);
        }
    }

    void Update()
    {
        if (isEnabled == false)
        {
            return;
        }

        if (unLocking)
        {
            Vector3 mousePos = canvas.transform.InverseTransformPoint(Input.mousePosition);
            lineOnEditRT.sizeDelta = new Vector2(lineOnEditRT.sizeDelta.x, Vector3.Distance(mousePos, circleOnEdit.transform.localPosition));
            lineOnEditRT.rotation = Quaternion.FromToRotation(Vector3.up, (mousePos - circleOnEdit.transform.localPosition).normalized);
        }
    }

    GameObject CreateLine(Vector3 pos, int id)
    {
        var line = GameObject.Instantiate(linePrefab, canvas.transform);

        line.transform.localPosition = pos;

        var lineCir = line.AddComponent<SliderCircle>();

        lineCir.id = id;
        lineCir.sliderLock = this;
        lines.Add(lineCir);

        return line;
    }

    void TrySetLineEdit(SliderCircle circle)
    {
        foreach (var line in lines)
        {
            if (line.id == circle.id)
            {
                return;
            }
        }

        lineOnEdit = CreateLine(circle.transform.localPosition, circle.id);
        lineOnEditRT = lineOnEdit.GetComponent<RectTransform>();
        circleOnEdit = circle;

        currentPattern.Add(circle.id); // 현재 패턴에 추가

        Manager.Sound.PlayButtonSound(14);
    }


    IEnumerator Release()
    {
        isEnabled = false;

        yield return new WaitForSeconds(2);

        foreach (var circle in circles)
        {
            circle.Value.GetComponent<Image>().color = Color.white;
            circle.Value.GetComponent<Animator>().enabled = false;
        }

        foreach (var line in lines)
        {
            Destroy(line.gameObject);
        }
        lines.Clear();

        lineOnEdit = null;
        lineOnEditRT = null;
        circleOnEdit = null;

        isEnabled = true;
        currentPattern.Clear(); // 현재 패턴 초기화
    }
    public void ClearRelease()
    {
        foreach (var line in lines)
        {
            Destroy(line.gameObject);
        }
    }

    void EnableColorFade(Animator anim, bool answer)
    {
        anim.enabled = true;
        anim.Rebind(); // 애니메이션을 처음부터 다시 시작
        if (answer)
        {
            anim.SetTrigger("Green");
        }
        else
        {
            anim.SetTrigger("Red");
        }
    }

    public void OnMouseEnterCircle(SliderCircle cir)
    {
        if (isEnabled == false)
        {
            return;
        }

        Debug.Log(cir.id);
        if (unLocking)
        {
            lineOnEditRT.sizeDelta = new Vector2(lineOnEditRT.sizeDelta.x, Vector3.Distance(lineOnEdit.transform.localPosition, cir.transform.localPosition));
            lineOnEditRT.rotation = Quaternion.FromToRotation(Vector3.up, (cir.transform.localPosition - circleOnEdit.transform.localPosition).normalized);

            TrySetLineEdit(cir);
        }
    }
    public void OnMouseExitCircle(SliderCircle cir)
    {
        if (isEnabled == false)
        {
            return;
        }
        Debug.Log(cir.id);
    }
    public void OnMouseDownCircle(SliderCircle cir)
    {
        if (isEnabled == false)
        {
            return;
        }
        Debug.Log(cir.id);
        unLocking = true;

        TrySetLineEdit(cir);
    }
    public void OnMouseUpCircle(SliderCircle cir)
    {
        if (isEnabled == false)
        {
            return;
        }
        Debug.Log(cir.id);

        if (unLocking)
        {
            bool patternCorrect = CheckPattern(); // 패턴 확인
            foreach (var line in lines)
            {
                EnableColorFade(circles[line.id].gameObject.GetComponent<Animator>(), patternCorrect);
            }

            Destroy(lines[lines.Count - 1].gameObject);
            lines.RemoveAt(lines.Count - 1);

            foreach (var line in lines)
            {
                EnableColorFade(line.GetComponent<Animator>(), patternCorrect);
            }
            if (!patternCorrect)
            {
                StartCoroutine(Release());
            }
            else
            {
                ChangeQuestion();
                clearMessage.SetActive(true);
                Manager.Chapter._clickObject.gameObject.SetActive(false);
            }
        }
        unLocking = false;
        
    }

    private bool CheckPattern()
    {
        if (currentPattern.Count != correctPattern.Count)
        {
            Debug.Log("패턴 불일치");
           
            return false;
        }

        for (int i = 0; i < currentPattern.Count; i++)
        {
            if (currentPattern[i] != correctPattern[i])
            {
                Debug.Log("패턴 불일치");
              
                return false;
            }
        }

        Debug.Log("패턴 일치");
        return true;
    }
    private void ChangeQuestion()
    {
        if(changeQuestion == true)
        {
            Manager.Chapter.HintData.SetClearQuestion(questionNum);
        }
    }
}
    