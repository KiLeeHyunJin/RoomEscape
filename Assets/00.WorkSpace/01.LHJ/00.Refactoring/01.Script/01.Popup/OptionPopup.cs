using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionPopup : PopUpUI
{
    enum Buttons
    {
        CloseButton,
        KoreanButtonOff,
        EnglishButtonOff,
        GoogleConnectButtonOff,
        TermsOfUse,
        PrivacyPolicy,
        Coupon,
        NicknameChange,
        ContactUs,
        Secession,
    }

    enum Images
    {
        BGMOffIcon,
        SFXOffIcon,

        EnglishButton,
        KoreanButton,

        GoogleConnectButton,
    }

    enum Sliders
    {
        BGMSlider,
        SFXSlider
    }

    [SerializeField] AudioMixer audioMixer;
    [SerializeField] string url = "https://m.naver.com/";
    Slider bgmSlider;
    Slider sfxSlider;

    Image bgmOffIcon;
    Image sfxmOffIcon;

    Image korean;
    Image english;

    Image googleConnect;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        float defaultValue = 50;
        //������ͼ��� ���� ������ �����̴��� ����
        bgmSlider.value = audioMixer.GetFloat("BGM", out float b_value) ? b_value : defaultValue;
        sfxSlider.value = audioMixer.GetFloat("SFX", out float s_value) ? s_value : defaultValue;
        googleConnect.gameObject.SetActive(false);
        ShowLanguage();
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindImage(typeof(Images));
        BindButton(typeof(Buttons));
        Bind<Slider>(typeof(Sliders));

        GetButton((int)Buttons.ContactUs).onClick.AddListener(() => Application.OpenURL(url));
        GetButton((int)Buttons.ContactUs).onClick.AddListener(() => Application.OpenURL(url));
        GetButton((int)Buttons.PrivacyPolicy).onClick.AddListener(() => Application.OpenURL(url));

        GetButton((int)Buttons.KoreanButtonOff).onClick.AddListener(ChangeLanguage);
        GetButton((int)Buttons.EnglishButtonOff).onClick.AddListener(ChangeLanguage);

        GetButton((int)Buttons.CloseButton).onClick.AddListener(Manager.UI.ClosePopUpUI);

        bgmSlider = Get<Slider>((int)Sliders.BGMSlider);
        sfxSlider = Get<Slider>((int)Sliders.SFXSlider);

        bgmSlider.onValueChanged.AddListener(delegate { SliderCheck(Sliders.BGMSlider); });
        sfxSlider.onValueChanged.AddListener(delegate { SliderCheck(Sliders.SFXSlider); });

        bgmOffIcon = GetImage((int)Images.BGMOffIcon);
        sfxmOffIcon = GetImage((int)Images.SFXOffIcon);

        korean = GetImage((int)Images.KoreanButton);
        english = GetImage((int)Images.EnglishButton);

        googleConnect = GetImage((int)Images.GoogleConnectButton);
        return true;
    }

    void ChangeLanguage()
    {
        Manager.Text._Iskr = !Manager.Text._Iskr;
        ShowLanguage();
    }

    void ShowLanguage()
    {
        bool koreanState = Manager.Text._Iskr;
        korean.gameObject.SetActive(koreanState);
        english.gameObject.SetActive(!koreanState);
    }

    void SliderCheck(Sliders sliderType)
    {
        (string sliderName, Slider slider, Image img) = sliderType switch
        {
            Sliders.BGMSlider => ("BGM", bgmSlider, bgmOffIcon),
            Sliders.SFXSlider => ("SFX", sfxSlider, sfxmOffIcon),
            _ => (null, null, null),
        };

        //���� -30 ���Ͽ��� ���������� ���ǹ��� �����̶� �Ǵ��Ͽ� ���Ұ� ó��(��� �׽�Ʈ �� ���� ���� ����)
        if (slider.value <= -30f)
        {
            audioMixer.SetFloat(sliderName, -80f);
            img.gameObject.SetActive(true);
        }
        //�����̴� �� �״�� ������ ����
        else
        {
            audioMixer.SetFloat(sliderName, slider.value);
            img.gameObject.SetActive(false);
        }
    }

}
