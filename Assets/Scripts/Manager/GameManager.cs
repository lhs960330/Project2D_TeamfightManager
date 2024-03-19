using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 얘가 게임에 필요한 스크립트(데이터)를 관리한다?
public class GameManager : Singleton<GameManager>
{
    // 데이터를 들고 있는 챔피언들 관리
    public List<ChampionData> championDatas;
    public ReStart restart;
    public int countRedteam;
    public int countBuleteam;
    public Canvas canvas;
    public int RedScore = 0;
    public int BuleScore = 0;
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

    public void GameEnd()
    {
        restart = FindAnyObjectByType<ReStart>();
        foreach (ChampionData champion in championDatas)
        {
            champion.animator.Play("Idle");
            if (champion.GetComponent<LongChampionController>() != null)
                champion.GetComponent<LongChampionController>().enabled = false;
            if (champion.GetComponent<ShortChampionController>() != null)
                champion.GetComponent<ShortChampionController>().enabled = false;


            if (champion.gameObject.GetComponentInChildren<arrowSpawn>() != null)
                Destroy(champion.gameObject.GetComponentInChildren<arrowSpawn>());
        }
        StartCoroutine(RestarRoutine());
    }
    IEnumerator RestarRoutine()
    {
        yield return new WaitForSeconds(3);
        Reset();
        restart.Restart?.Invoke();

    }
    public void Reset()
    {
        foreach (ChampionData champion in championDatas)
        {
            Destroy(champion.gameObject);
        }
    }
    /*    public void KillScore()
        {

        }*/

    public int GetRedScore()
    {
        return RedScore;
    }
    public int GetBuleScore()
    {
        return BuleScore;
    }
    public void SetRedScore()
    {
        ++RedScore;
    }
    public void SetBuleScore()
    {
        ++BuleScore;
    }



}

