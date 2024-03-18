using UnityEngine;
using UnityEngine.UI;


public class TeamPick : MonoBehaviour
{
    [SerializeField] ChampionData prefab;
    [SerializeField] Image pick;

    public ChampionData Champion { get { return prefab; } }
    private void Awake()
    {
        prefab.Team = 3;
    }
    public void PlayerPick()
    {
        if (prefab.Team == 1)
            return;
        prefab.Team = 0;
    }

    public void ComPick()
    {       
        prefab.Team = 1;
        pick.gameObject.SetActive(true);
    }
}
