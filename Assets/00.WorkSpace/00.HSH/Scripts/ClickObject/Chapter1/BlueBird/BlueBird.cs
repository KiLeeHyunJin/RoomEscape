using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlueBird : ClickObject
{
    [SerializeField] PlayInGameUI _playInGameUI;
    [SerializeField] RectTransform _Content;
    [SerializeField] GameObject cage1;
    [SerializeField] GameObject cage2;
    [SerializeField] GameObject cage3;
    [SerializeField] bool offCheck = true;
    [SerializeField] bool stateCheck = true;
    [SerializeField] bool stateCheck2 = true;
    [SerializeField] int Sentence2;

    private void Start()
    {
        if (state == 0 && Manager.Chapter.ContinueState == false)
        {
            Vector2 _position = new Vector2(475, 0);
            //(transform.parent.transform as RectTransform).anchoredPosition = _position;
            if(_Content != null)
            _Content.anchoredPosition = _position;
            LogPlay(sentenceNum);
            state = 1;
        }
    }

    private void Update()
    {
        if (offCheck)
        {
            offCheck =false;
            cage1.SetActive(false);
            cage2.SetActive(false);
        }

        if(state == 2 && stateCheck)
        {
            stateCheck = false;
            cage3.SetActive(false);
            cage2.SetActive(true);
        }

        if (state == 3 && stateCheck2)
        {
            stateCheck2 = false;
            cage3.SetActive(false);
            cage2.SetActive(false);
            cage1.SetActive(true);
        }
    }

    public override void ClickEffect()
    {
        switch(state)
        {
            case 0:
                return;
            case 1:
                Interact();
                if (state == 1)
                {
                    Manager.Sound.PlayButtonSound(0);
                    LogPlay(Sentence2);
                }
                break;
            case 2:
                GetItem(item);
                state = 3;
                Manager.Sound.PlayButtonSound(4);
                return;
             case 3:
                PopUp();
                Manager.Sound.PlayButtonSound(0);
                break;
        }
    }
    public override void InteractEffect()
    {
        state = 2;
        Manager.Chapter.HintData.SetClearQuestion(4);
        Manager.Sound.PlayButtonSound(34);
    }
}
