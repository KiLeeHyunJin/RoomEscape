public class pocketGame : JigsawPuzzle
{
    //여기서의 클릭오브젝트는 소상인
    public void GameEnd()
    {
        Manager.Chapter._clickObject.state = 0;
        Manager.Chapter._clickObject.GetItem(Manager.Chapter._clickObject.item);
        Close();
    }
}
