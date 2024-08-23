using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutoirialAds : MonoBehaviour
{
    [SerializeField] GameObject tutorialAds;
    [SerializeField] Button closeButton;
    public TextMeshProUGUI adsText;
    public StringBuilder duringString;
    private int inGameTime = 10;
    GameManager gameManager;

    private void Start()
    {
        Message.Log("광고 시작");
        StartCoroutine(ShowAdsAfterDelay(10));
        closeButton.onClick.AddListener(OffAds);
    }

    public void OnAds()
    {
        tutorialAds.SetActive(true);
        gameManager.TimerStart(inGameTime);
        StartCoroutine(HideAdsAfterDuration());
    }

    public void OffAds()
    {
        tutorialAds.SetActive(false);
    }

    IEnumerator HideAdsAfterDuration()
    {
        yield return new WaitForSeconds(10);
        OffAds();
    }

    IEnumerator ShowAdsAfterDelay(int delay)
    {
        yield return new WaitForSeconds(delay);
        OnAds();
    }
}
