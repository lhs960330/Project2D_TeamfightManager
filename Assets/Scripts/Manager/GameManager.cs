using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

// �갡 ���ӿ� �ʿ��� ��ũ��Ʈ(������)�� �����Ѵ�?
public class GameManager : Singleton<GameManager>
{
    // �����͸� ��� �ִ� è�Ǿ�� ����
    public ChampionData[] championDatas;
   
    protected override void Awake()
    {
        base.Awake();
        
        championDatas = new ChampionData[10];

    }

    public void Start()
    {
        // ��� è�Ǿ� �����͸� ������ 
        championDatas = FindObjectsOfType<ChampionData>();
    }
}

