using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Test : MonoBehaviour
{
   
    [SerializeField] int hp;
    [SerializeField] int range;
    [SerializeField] int damage;
    [SerializeField] int speed;
    [SerializeField] int attackTime;

    [SerializeField] GameObject Enemy;

    private void Start()
    {
        
    }

    private void Update()
    {
        Vector3 enemyPos = Enemy.transform.position;

        Vector3 dir = (enemyPos - transform.position).normalized;
        transform.Translate(dir * speed * Time.deltaTime);

        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Champion"))
        {
            hitRoutine = StartCoroutine(HitRoutine());
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Champion"))
        {
            Debug.Log("½Î¿ò½ÃÀÛ");
            StopCoroutine(hitRoutine);
        }
    }

    Coroutine hitRoutine;

    IEnumerator HitRoutine()
    {
        while (true)
        {
            hp -= Enemy.GetComponent<Test>().damage;
            yield return new WaitForSeconds(attackTime);
        }
    }
}
