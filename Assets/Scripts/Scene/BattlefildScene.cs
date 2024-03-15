using System.Collections;
using TMPro;
using UnityEngine;

public class BattlefildScene : BaseScene
{
    [SerializeField] public TMP_Text RedKill;
    [SerializeField] public TMP_Text BlueKill;
    public override IEnumerator LoadingRoutine()
    {
        Manager.Game.SetData();
        yield return null;
    }


}
