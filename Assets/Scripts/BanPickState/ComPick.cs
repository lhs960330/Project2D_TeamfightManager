using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComPick : MonoBehaviour
{
    [SerializeField] List<GameObject> prefabs;

    // Start is called before the first frame update
    void Start()
    {
        prefabs = new List<GameObject>();

    }

    // Update is called once per frame
    void Update()
    {
        int com = Random.Range(0, prefabs.Count);

    }
}
