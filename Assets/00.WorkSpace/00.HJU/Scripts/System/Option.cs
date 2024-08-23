using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static UserGameData;

public class Option : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;

    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;

    [SerializeField] GameObject bgmOffIcon;
    [SerializeField] GameObject sfxmOffIcon;

    [SerializeField] GameObject TitleText;

    private void OnEnable()
    {
        audioMixer = Resources.Load<AudioMixer>("AudioMixer");

        //슬라이더에 오디오믹서 변경 함수 적용
        bgmSlider.onValueChanged.AddListener(delegate { SliderCheck(bgmSlider); });
        sfxSlider.onValueChanged.AddListener(delegate { SliderCheck(sfxSlider); });
        //오디오믹서의 현재 음량을 슬라이더에 적용
        audioMixer.GetFloat("BGM", out float b_value);
        bgmSlider.value = b_value;
        audioMixer.GetFloat("SFX", out float s_value);
        sfxSlider.value = s_value;

        TitleCheck();
    }

    private void OnDisable()
    {
        audioMixer.GetFloat("BGM", out float b_value);
        Manager.Data.UserGameData.SetData(GameDataEnum.Bgm, b_value);
        audioMixer.GetFloat("SFX", out float s_value);
        Manager.Data.UserGameData.SetData(GameDataEnum.Sfx, s_value);
    }

    /// <summary>
    /// 슬라이더의 변경값을 오디오믹서에 적용
    /// </summary>
    public void SliderCheck(Slider slider)
    {
        //볼륨 -30 이하에서 볼륨조절이 무의미한 수준이라 판단하여 음소거 처리(기기 테스트 후 범위 변경 예정)
        if (slider.value <= -30f)
        {
            if (slider == bgmSlider)
            {
                audioMixer.SetFloat("BGM", -80f);
                bgmOffIcon.SetActive(true);
            }
            else
            {
                audioMixer.SetFloat("SFX", -80f);
                sfxmOffIcon.SetActive(true);
            }

        }
        //슬라이더 값 그대로 볼륨에 적용
        else
        {
            if (slider == bgmSlider)
            {
                audioMixer.SetFloat("BGM", slider.value);
                bgmOffIcon.SetActive(false);
            }

            else
            {
                audioMixer.SetFloat("SFX", slider.value);
                sfxmOffIcon.SetActive(false);
            }
        }
    }

    public void ClearCached()
    {
        Caching.ClearCache();
    }

    /// <summary>
    /// 웹페이지 오픈 - 인스펙터에서 주소 입력 
    /// </summary>
    public void OnClickOpenURL(string URL)
    {
        Application.OpenURL(URL);
    }
    // 타이틀 제목 자동으로 읽어오는 스크립트
    private void TitleCheck()
    {
        if (TitleText == null)
        {
            Manager.Text.TextChange();
            return;
        }
        else
        {
            switch (Manager.Chapter.chapter)
            {
                case 0:
                    TitleText.name = "1000";
                    break;
                case 1:
                    TitleText.name = "2000";
                    break;
                case 2:
                    TitleText.name = "3000";
                    break;
                case 3:
                    TitleText.name = "4000";
                    break;
                case 4:
                    TitleText.name = "5000";
                    break;
                case 5:
                    TitleText.name = "6000";
                    break;
            }
            Manager.Text.TextChange();
        }
    }
    public void LobbyScene()
    {
        Manager.Data.UserGameData.SetData(GameDataEnum.Chapter, -1);
        Manager.Game.SetGiveUp();
        Manager.UI.ClearPopUpUI();
        Manager.Inventory.Init();
        Manager.Game.ShowNewAbs();
        Manager.Resource.ReleaseBundle();
        Manager.Scene.LoadScene("LobbyScene");
    }
}
