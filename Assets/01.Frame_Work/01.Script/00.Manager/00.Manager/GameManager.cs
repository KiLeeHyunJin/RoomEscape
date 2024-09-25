using System.Collections;
using System.Text;
using UnityEngine;
using Unity;

public class GameManager : Singleton<GameManager>
{



    public void Vibration()
    {
#if UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }

}
