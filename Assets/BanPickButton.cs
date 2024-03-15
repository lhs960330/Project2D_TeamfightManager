using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BanPickButton : MonoBehaviour
{
    List<Battle> battle;
    private void Awake()
    {
        battle = new List<Battle>();
    }
    private void Start()
    {
        battle = FindObjectsOfType<Battle>().ToList();
    }
    public void Actives()
    {
        foreach (Battle go in battle)
        {
            go.gameObject.SetActive(true);
        }
    }
}
