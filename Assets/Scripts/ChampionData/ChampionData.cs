using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class ChampionData : MonoBehaviour
{
    [SerializeField] public int maxHp;
    public int hp;
    [SerializeField] public int speed;
    [SerializeField] public float range;
    [SerializeField] public int attackTime;
    [SerializeField] public int damage;
    [SerializeField] public float avoidrange;

    public float avoidcool = 0;
    public int respawn = 5;
    public Transform my;
    [SerializeField] public Animator animator;

    private void Awake()
    {
        avoidrange = range - 1;
        hp = maxHp;
        gameObject.transform.position = my.position;
    }
   
}
