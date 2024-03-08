using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// �갡 ���ӿ� �ʿ��� ��ũ��Ʈ(������)�� �����Ѵ�?
public class GameManager : Singleton<GameManager>
{
    // �����͸� ��� �ִ� è�Ǿ�� ����
    public ChampionData[] championDatas;
    // �����͸� ��� �ִ� è�Ǿ����  ������ ���� (true : Red, false : Bule)
    public Dictionary<bool, ChampionData[]> championsdata;
    // �ٰŸ� ���Ÿ� ������ ����
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
        // �����͸� ����ִ� è�Ǿ���� ã�Ƽ� �־���
        // ��� è�Ǿ� �����͸� ������ 
        championDatas = FindObjectsOfType<ChampionData>();
        //���Ÿ� è�Ǿ�����
        longChampion = FindObjectsOfType<LongChampionController>();
        // �ٰŸ� ������
        shortChampion = FindObjectsOfType<ShortChampionController>();

        // ��� è�Ǿ� �����͸� ����� ���� ������
        ChampionData[] Ateam = new ChampionData[10];
        ChampionData[] Bteam = new ChampionData[10];


        // ������ �־���(���з��� ���⼭�ؾߵǳ�?)
        for (int i = 0; i < championDatas.Length; i++)
        {
            if (championDatas[i] == null) continue;
            // ����
            if (championDatas[i].team == true)
            {
                Ateam[AIndex] = championDatas[i];
                AIndex++;
            }
            // ���
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

