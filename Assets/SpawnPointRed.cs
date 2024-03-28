using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointRed : MonoBehaviour
{
    [SerializeField] List<ChampionData> prefabs;
    int red = 0;
    private BoxCollider2D SpawnSize;
    float posX;
    float posY;
    Vector3 BasePos;
    Vector3 Size;
    Vector3 SpawnPos;

    private void OnEnable()
    {
        SpawnSize = GetComponent<BoxCollider2D>();
        BasePos = transform.position;
        Size = SpawnSize.size;

        foreach (var prefab in prefabs)
        {
            posX = BasePos.x + UnityEngine.Random.Range(-Size.x / 2f, Size.x / 2f);
            posY = BasePos.y + UnityEngine.Random.Range(-Size.y / 2f, Size.y / 2f);
            SpawnPos = new Vector3(posX, posY, 0);

            if (prefab.Team == red)
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
            foreach (var prefab in prefabs)
            {
                posX = BasePos.x + UnityEngine.Random.Range(-Size.x / 2f, Size.x / 2f);
                posY = BasePos.y + UnityEngine.Random.Range(-Size.y / 2f, Size.y / 2f);
                SpawnPos = new Vector3(posX, posY, 0);
                if (GameObject.Find(prefab.name + "(Clone)") == null && prefab.Team == red)
                {
                    yield return new WaitForSeconds(5);
                    ChampionData newObject = Instantiate(prefab, SpawnPos, Quaternion.identity);
                    //Manager.Game.ChampionDataProduce(newObject);
                }
                else
                {
                    yield return null;
                }
            }
        }
    }
}

