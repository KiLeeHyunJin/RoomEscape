using System.IO;
using UnityEngine;
using UnityEngine.Android;

public class DirectoryCheck : MonoBehaviour
{
    /// <summary>
    /// 권한이 없을 경우 권한 요청 후 디렉터리 체크. 디렉터리가 없을 경우 디렉터리 생성
    /// </summary>
    public void CheckDirec()
    {
        if (Manager.Data.HasPermission(Permission.ExternalStorageRead) == false)
        {
            Manager.Data.RequestPermissions();
            return;
        }

        PopUpUI popup = Manager.UI.ShowPopUpUI("ShowResult");
        if (popup == null)
            return;

        StatePopup state = popup as StatePopup;
        if (state == null)
            return;

        string checkPath = $"{Manager.Data.DataPath}";


        if (System.IO.Directory.Exists(checkPath))
        {
            state.txt.text = $"True \n {checkPath}";
        }
        else
        {
            state.txt.text = $"False \n {checkPath}";
            Directory.CreateDirectory(checkPath);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void CheckUserData(string info)
    {
        PopUpUI popup = Manager.UI.ShowPopUpUI("ShowResult");
        if (popup == null)
            return;

        StatePopup state = popup as StatePopup;
        if (state == null)
            return;
        state.txt.text = info;
    }

}
