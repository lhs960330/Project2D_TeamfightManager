using UnityEngine;


public class ChampionData : MonoBehaviour
{
    [SerializeField] public int Team;
    [SerializeField] public int maxHp;
    public int hp;
    [SerializeField] public int speed;
    [SerializeField] public float range;
    [SerializeField] public int attackTime;
    [SerializeField] public int damage;
    public int respawn = 5;
    public Transform my;
    [SerializeField] public Animator animator;

    private void Awake()
    {
        hp = maxHp;
        gameObject.transform.position = my.position;
    }
}
