using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// �갡 ���ӿ� �ʿ��� ��ũ��Ʈ(������)�� �����Ѵ�?
public class GameManager : Singleton<GameManager>
{
    // �����͸� ��� �ִ� è�Ǿ�� ����
    public List<ChampionData> championDatas;
    public int countAteam;
    public int countBteam;

    protected override void Awake()
    {
        base.Awake();

        championDatas = new List<ChampionData>();
    }

    public void Start()
    {
        // ��� è�Ǿ� �����͸� ������ 
        championDatas = FindObjectsOfType<ChampionData>().ToList();
        foreach(ChampionData champion in championDatas)
        {
            if (champion.Team == 0)
                countAteam++;
            else
                countBteam++;
        }
    }
    private void Update()
    {
    }

    public void RemoveChampion(ChampionData champion)
    {
        if (champion.Team == 0)
            countAteam--;
        else
            countBteam--;
        championDatas.Remove(champion);
    }
}

