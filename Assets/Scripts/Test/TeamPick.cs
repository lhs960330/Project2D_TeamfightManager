using UnityEngine;

public class TeamPick : MonoBehaviour
{
    [SerializeField] ChampionData prefab;
    private void Awake()
    {
        ComPick();
    }
    public void PlayerPick()
    {
        prefab.Team = 0;
    }

    public void ComPick()
    {
        prefab.Team = 1;
    }
    private void OnDisable()
    {
        if (prefab.Team != 0)
            ComPick();
    }
}
