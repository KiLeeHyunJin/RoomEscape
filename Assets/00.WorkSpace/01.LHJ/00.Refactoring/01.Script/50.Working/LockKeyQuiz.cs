using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class LockKeyQuiz : PopUpUI
{
    enum Buttons
    {
        CloseButton,
    }

    public enum QuizeLanguageType
    {
        Number,
        Alphabet,
    }

    public string[] numbers; // 표시할 문자열 배열
    [Header("ConnectObject")]
    [SerializeField] public GameObject nextObject;

    [Header("answer")]
    [SerializeField] public string[] answers;


    [Header("EndPhase")]
    [SerializeField] ScriptableItem treasure;
    [SerializeField] public GameObject clearMessage;
    [SerializeField] int changeState;
    [SerializeField] int setCount; //조각


    int costValue;  //기회
    int quizSize;   //범위

    GridLayoutGroup layout;
    List<(Button btn, TextMeshProUGUI txt)> gameObjList;
    QuizeLanguageType languageType;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        layout.transform.localScale = gameObjList[0].btn.transform.localScale;
        float sellSize = (gameObjList[0].btn.transform as RectTransform).rect.width * 0.3f;
        layout.spacing = new Vector2(0, sellSize);

        foreach (var (btn, txt) in gameObjList)
        {
            btn.transform.localScale = Vector3.one;
            txt.text = numbers[0];
        }
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        costValue = costValue == 0 ? -1 : costValue;
        quizSize = quizSize <= 0 ? 10 : quizSize;
        setCount = setCount <= 0 ? 3 : setCount;
        gameObjList = new();
        SetLanguage();

        layout = GetComponentInChildren<GridLayoutGroup>();
        Transform grid = layout.transform;
        Button diaNumBox = Manager.Resource.Load<Button>($"UI/Popup/Prefab/DialNumBox");
        if(diaNumBox == null)
        {

        }
        for (int i = 0; i < setCount; i++)
        {
            Button btn = Instantiate<Button>(diaNumBox);
            btn.name = $"Number{i}";
            btn.transform.SetParent(grid);
            gameObjList.Add((
                btn, 
                btn.GetComponentInChildren<TextMeshProUGUI>()));
        }

        foreach (var (btn, txt) in gameObjList)
        {
            btn.onClick.AddListener(() =>
            {
                if (costValue == 0)
                {
                    Over();
                    return;
                }
                    
                costValue--;
                int index = languageType == QuizeLanguageType.Number ? 
                    int.Parse(txt.text) + 1 : (txt.text[0] + 1) - 'A';
                index %= numbers.Length;
                txt.text = numbers[index];
                CheckAnswer();
            });
        }

        BindButton(typeof(Buttons));
        GetButton((int)Buttons.CloseButton).onClick.AddListener(Manager.UI.ClosePopUpUI);

        return true;
    }

    void Over()
    {

    }



    public void CheckAnswer()
    {
        for (int i = 0; i < answers.Length; i++)
        {
            if (((int)answers[i][0] == (int)gameObjList[i].txt.text[0]) ==  false)
            {
                return;
            }
        }

        if (Manager.Chapter._clickObject != null)
        {
            if (Manager.Chapter._clickObject.item != null)
            {
                Manager.Chapter._clickObject.GetItem(Manager.Chapter._clickObject.item);
            }
        }
        if (treasure != null)
        {
            Manager.Inventory.ObtainItem(treasure);
        }

        if (clearMessage != null)
        {
            clearMessage.SetActive(true);
        }

        Manager.Chapter._clickObject?.ChangeImage();
        Manager.Chapter._clickObject.state = changeState;
    }

    void SetLanguage()
    {
        numbers = new string[quizSize];
        if (languageType == QuizeLanguageType.Number)
        {
            for (int i = 0; i < quizSize; i++)
            {
                numbers[i] = i.ToString();
            }
        }
        else
        {
            for (int i = 0; i < quizSize; i++)
            {
                numbers[i] = ((char)('A' + i)).ToString();
            }
        }
    }

    public void SetInit(int count, QuizeLanguageType type = QuizeLanguageType.Number, int size = 10, int cost = -1)
    {
        setCount = count;
        languageType = type;
        costValue = cost;
        quizSize = size;
        Init();
    }
}
