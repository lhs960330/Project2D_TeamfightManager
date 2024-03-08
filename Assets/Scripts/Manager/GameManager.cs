using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// 얘가 게임에 필요한 스크립트(데이터)를 관리한다?
public class GameManager : Singleton<GameManager>
{
    // 데이터를 들고 있는 챔피언들 관리
    public ChampionData[] championDatas;
    // 데이터를 들고 있는 챔피언들을  팀으로 관리 (true : Red, false : Bule)
    public Dictionary<bool, ChampionData[]> championsdata;
    // 근거리 원거리 나눠서 관리
    public LongChampionController[] longChampion;
    public ShortChampionController[] shortChampion;

    int AIndex = 0;
    int BIndex = 0;
    protected override void Awake()
    {
        base.Awake();
        
        championDatas = new ChampionData[10];

        championsdata = new Dictionary<bool, ChampionData[]>();
        longChampion = new LongChampionController[10];
        shortChampion = new ShortChampionController[10];
    }

    public void Start()
    {
        // 데이터를 들고있는 챔피언들을 찾아서 넣어줌
        // 모든 챔피언 데이터를 가져옴 
        championDatas = FindObjectsOfType<ChampionData>();
        //원거리 챔피언가져옴
        longChampion = FindObjectsOfType<LongChampionController>();
        // 근거리 가져옴
        shortChampion = FindObjectsOfType<ShortChampionController>();

        // 모든 챔피언 데이터를 레드와 블루로 나눠줌
        ChampionData[] Ateam = new ChampionData[10];
        ChampionData[] Bteam = new ChampionData[10];


        // 팀으로 넣어줌(팀분류를 여기서해야되나?)
        for (int i = 0; i < championDatas.Length; i++)
        {
            if (championDatas[i] == null) continue;
            // 레드
            if (championDatas[i].team == true)
            {
                Ateam[AIndex] = championDatas[i];
                AIndex++;
            }
            // 블루
            else if (championDatas[i].team == false)
            {
                Bteam[BIndex] = championDatas[i];
                BIndex++;
            }
        }
        championsdata.Add(true, Ateam);
        championsdata.Add(false, Bteam);

    }
}

