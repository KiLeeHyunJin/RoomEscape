using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISound : MonoBehaviour
{
    [SerializeField] AudioClip buttonClick;
    [SerializeField] AudioClip bgm;

    private void Awake()
    {
        Bind();
    }
    private void Start()
    {
        Manager.Sound.PlayBGM(bgm);
    }

    private void Bind()
    {
        Component[] components = GetComponentsInChildren<Component>(true);

        foreach (Component child in components)
        {

            if (child is Button button)
            {
                button.onClick.AddListener(ButtonSoundPlay);
            }
        }
    }

    public void ButtonSoundPlay()
    {
        Manager.Sound.PlaySFX(buttonClick);
    }
}
