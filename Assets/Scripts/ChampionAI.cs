using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class ChampionAI : MonoBehaviour
{
    [SerializeField] int hp;
    [SerializeField] int Damage;
    [SerializeField] int moveSpeed;

    [SerializeField] ChampionAI championAI;
    [SerializeField] Transform championTransform;

    private void Start()
    {
        championTransform = championAI.gameObject.transform;
    }
    private void Update()
    {
        Vector3 playerPos = championTransform.position;


        Vector3 dir = (playerPos - transform.position).normalized;
        transform.Translate(dir * moveSpeed * Time.deltaTime);
        if(hp <= 0)
        {
            Destroy(gameObject);
        }
    }
    public void OnTriggerEnter()
    {
        championAI.hp -= Damage;
    }
    public void OnTriggerStay()
    {
        HitRoutine = StartCoroutine(HitsRoutine());
    }

    public void OnTriggerExit()
    {
        StopCoroutine(HitRoutine);
    }
    Coroutine HitRoutine;

    IEnumerator HitsRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        championAI.hp -= Damage;
    }


}
