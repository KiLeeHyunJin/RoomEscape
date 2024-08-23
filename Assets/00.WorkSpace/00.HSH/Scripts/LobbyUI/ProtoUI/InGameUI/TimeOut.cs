public class TimeOut : PopUpUI
{
    private void OnEnable()
    {
        Manager.Game.Pause();
    }
    private void OnDisable()
    {
        Manager.Game.Resume();
    }
}
