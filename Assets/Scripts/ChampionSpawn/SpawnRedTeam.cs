using System.Collections.Generic;
using UnityEngine;

public class SpawnRedTeam : ChampionSpawn
{
    List<ChampionData> SpawnRedTeamData = new List<ChampionData>();
    private void Awake()
    {
        Manager.Game.ChampionDataProduce();
    }
    void Start()
    {
        foreach (var data in prefabs)
        {
            if (data.Team == 0)
            {
                SpawnRedTeamData.Add(data);
                Instantiate(data, transform.position, Quaternion.identity);
            }
        }
    }
}
