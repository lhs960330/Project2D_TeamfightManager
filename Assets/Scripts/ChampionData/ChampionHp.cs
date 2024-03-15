using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChampionHp : MonoBehaviour
{
   [SerializeField] GameObject prefab;
   [SerializeField] GameObject canvas;

    RectTransform hpBar;

    public float height = 1.7f;

    private void Start()
    {
        hpBar = Instantiate(prefab, canvas.transform).GetComponent<RectTransform>();
    }

    private void Update()
    {
        Vector3 hpBarPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + height,0));
        hpBar.position = hpBarPos;
    }
}
