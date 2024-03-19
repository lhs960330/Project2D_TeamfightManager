using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChampionHp : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Image colorHp;
    [SerializeField] ChampionData data;
    protected float curHp;
    public float maxHp;

    private void Awake()
    {
        if(data.Team == 0)
        {
            colorHp.color = Color.red;
        }
        else if(data.Team == 1)
        {
            colorHp.color = Color.green;
        }
        maxHp = data.maxHp;
    }

    private void Update()
    {
        curHp = data.hp;
        slider.value = curHp/maxHp;
    }

}
