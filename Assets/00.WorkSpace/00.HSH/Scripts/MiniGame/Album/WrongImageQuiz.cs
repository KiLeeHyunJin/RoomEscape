using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WrongImageQuiz : MonoBehaviour
{
    [Header("Change")]
    public bool _Founded;
    [SerializeField] WrongImageGame MotherGame;
    public GameObject[] GreenCircle;
    //발견됨!
    public void Founded()
    {
        if (_Founded == false)
        {
            MotherGame.foundCount++;
            _Founded = true;
            GreenCircleTurnOn();
            MotherGame.CheckAnswer();
            Manager.Sound.PlayButtonSound(41);
        }
        else
        {
            return;
        }
        
    }
    private void GreenCircleTurnOn()
    {
        for (int i = 0; i < GreenCircle.Length; i++)
        {
            GreenCircle[i].SetActive(true);
        }
    }
}