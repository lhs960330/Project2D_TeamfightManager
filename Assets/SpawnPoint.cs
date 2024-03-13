using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] List<ChampionData> prefabs;
    int bule = 1;
    int count = 2;
    private void Awake()
    {
        foreach (var prefab in prefabs)
        {
            if (prefab.Team == bule)
            {
                Instantiate(prefab, transform.position, Quaternion.identity);
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
                if (GameObject.Find(prefab.name + "(Clone)") == null && prefab.Team == bule)
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
