using System.Collections.Generic;
using UnityEngine;

public class SpawnBuleTeam : ChampionSpawn
{
     List<ChampionData> SpawnBlueTeamData=new List<ChampionData>();
    void Start()
    {
        foreach (var data in prefabs)
        {
            if (data.Team == 1)
            {
                SpawnBlueTeamData.Add(data);
            }
        }
    }

    void Update()
    {
        foreach(var data in SpawnBlueTeamData)
        {
            if(data == null)
            {
                Instantiate(data, transform.position, Quaternion.identity);
            }
        }
    }
}
