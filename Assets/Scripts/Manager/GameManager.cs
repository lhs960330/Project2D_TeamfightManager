using System.Collections.Generic;
using System.Linq;

// 얘가 게임에 필요한 스크립트(데이터)를 관리한다?
public class GameManager : Singleton<GameManager>
{
    // 데이터를 들고 있는 챔피언들 관리
    public List<ChampionData> championDatas;
    public int countRedteam;
    public int countBuleteam;

    protected override void Awake()
    {
        base.Awake();

        championDatas = new List<ChampionData>();
    }

    public void SetData()
    {
        // 모든 챔피언 데이터를 가져옴 
        championDatas = FindObjectsOfType<ChampionData>().ToList();
        foreach (ChampionData champion in championDatas)
        {
            if (champion.Team == 0)
                countRedteam++;
            else
                countBuleteam++;
        }
    }
    public void ChampionDataProduce(ChampionData cham)
    {
            if (cham.Team == 0)
                countRedteam++;
            else
                countBuleteam++;

        championDatas.Add(cham);
    }
    public void RemoveChampion(ChampionData champion)
    {
        if (champion.Team == 0)
            countRedteam--;
        else
            countBuleteam--;
        championDatas.Remove(champion);

    }
}

