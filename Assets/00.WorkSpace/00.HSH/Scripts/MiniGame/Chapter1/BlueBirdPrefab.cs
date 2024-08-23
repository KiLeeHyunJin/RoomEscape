using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueBirdPrefab : PopUpUI
{
    [SerializeField] GameObject[] musics;
    //[SerializeField] GameObject blocker;
    [SerializeField] Animator blueBirdAnimator;
    public void Sing()
    {
        StartCoroutine(SummonSing());
    }

    public void SingSound()
    {
        Manager.Sound.PlayButtonSound(35);
    }

    IEnumerator SummonSing()
    {
        //blocker.SetActive(true);


        //for (int i = 0; i < musics.Length; i++)
        //{
        //    musics[i].SetActive(true);
        blueBirdAnimator.SetTrigger("start");
        yield return new WaitForSeconds(3);
        //    musics[i].SetActive(false);
        //}
        //blocker.SetActive(false);
    }
}
