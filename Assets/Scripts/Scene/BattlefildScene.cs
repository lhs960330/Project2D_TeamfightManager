using System.Collections;

public class BattlefildScene : BaseScene
{
    public override IEnumerator LoadingRoutine()
    {
        Manager.Game.SetData();
        yield return null;
    }
}
