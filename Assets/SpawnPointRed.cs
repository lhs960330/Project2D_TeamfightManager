using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointRed : MonoBehaviour
{
    [SerializeField] List<ChampionData> prefabs;
    int red = 0;
    private void Awake()
    {
        foreach (var prefab in prefabs)
        {
            if (prefab.Team == red)
            {
                Instantiate(prefab, transform.position, Quaternion.identity);
            }
        }
        StartCoroutine(RespawnRutine());
    }
    IEnumerator RespawnRutine()
    {
        int loop = 0;
        while (true)
        {
            loop++;
            if (loop > 10000)
                throw new InvalidOperationException("A");
            foreach (var prefab in prefabs)
            {
                if (GameObject.Find(prefab.name + "(Clone)") == null && prefab.Team == red)
                {
                    yield return new WaitForSeconds(5);
                    ChampionData newObject = Instantiate(prefab, transform.position, Quaternion.identity);
                    Manager.Game.ChampionDataProduce(newObject);
                }
                else
                {
                    yield return null;
                }
            }
        }
    }
}

