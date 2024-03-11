using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// 얘가 게임에 필요한 스크립트(데이터)를 관리한다?
public class GameManager : Singleton<GameManager>
{
    // 데이터를 들고 있는 챔피언들 관리
    public ChampionData[] championDatas;
   
    protected override void Awake()
    {
        base.Awake();
        
        championDatas = new ChampionData[10];

    }

    public void Start()
    {
        // 모든 챔피언 데이터를 가져옴 
        championDatas = FindObjectsOfType<ChampionData>();
    }
}

