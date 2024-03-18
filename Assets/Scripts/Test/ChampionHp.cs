using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChampionHp : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] ChampionData data;
    protected float curHp;
    public float maxHp;

    private void Awake()
    {
        maxHp = data.maxHp;
    }

    private void Update()
    {
        curHp = data.hp;
        slider.value = curHp/maxHp;
    }

}
