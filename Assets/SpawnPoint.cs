using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] List<ChampionData> prefabs;
    int bule = 1;
    private BoxCollider2D SpawnSize;
    float posX;
    float posY;
    Vector3 BasePos;
    Vector3 Size;
    Vector3 SpawnPos;
    private void Awake()
    {
        SpawnSize = GetComponent<BoxCollider2D>();
        BasePos = transform.position;
        Size = SpawnSize.size;
        foreach (var prefab in prefabs)
        {
            posX = BasePos.x + UnityEngine.Random.Range(-Size.x / 2f, Size.x / 2f);
            posY = BasePos.y + UnityEngine.Random.Range(-Size.y / 2f, Size.y / 2f);
            SpawnPos = new Vector3(posX, posY, 0);
            if (prefab.Team == bule)
            {
                Instantiate(prefab, SpawnPos, Quaternion.identity);
            }
        }
        StartCoroutine(RespawnRutine());
    }
    IEnumerator RespawnRutine()
    {
        while (true)
        {
            posX = BasePos.x + UnityEngine.Random.Range(-Size.x / 2f, Size.x / 2f);
            posY = BasePos.y + UnityEngine.Random.Range(-Size.y / 2f, Size.y / 2f);
            SpawnPos = new Vector3(posX, posY, 0);
            foreach (var prefab in prefabs)
            {
                if (GameObject.Find(prefab.name + "(Clone)") == null && prefab.Team == bule)
                {
                    yield return new WaitForSeconds(5);
                    ChampionData newObject = Instantiate(prefab, SpawnPos, Quaternion.identity);
                   // Manager.Game.ChampionDataProduce(newObject);
                }
                else
                {
                    yield return null;
                }
            }
        }
    }

}
